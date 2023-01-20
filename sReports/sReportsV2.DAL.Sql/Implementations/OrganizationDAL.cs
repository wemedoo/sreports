using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.SqlDomain.Filter;
using System.Data.Entity;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Helpers;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class OrganizationDAL : IOrganizationDAL
    {
        private readonly SReportsContext context;

        public OrganizationDAL(SReportsContext sReportsContext)
        {
            this.context = sReportsContext;
        }

        public void Delete(Organization organization)
        {
            Organization fromDb = context.Organization.FirstOrDefault(x => x.OrganizationId == organization.OrganizationId);
            DoDeleteCheck(fromDb);
            if (fromDb != null)
            {
                context.Entry(fromDb).OriginalValues["RowVersion"] = organization.RowVersion;
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public bool ExistIdentifier(Identifier identifier)
        {
            return context.Identifiers.Any(x => x.System.Equals(identifier.System) && x.Value.Equals(identifier.Value));
        }

        public List<Organization> GetAll(OrganizationFilter organizationFilter)
        {
            IQueryable<Organization> result = GetOrganizationsFiltered(organizationFilter);
            
            if (organizationFilter.ColumnName != null) 
                result = SortByField(result, organizationFilter);
            else
                result = result.OrderByDescending(x => x.OrganizationId)
                    .Skip((organizationFilter.Page - 1) * organizationFilter.PageSize)
                    .Take(organizationFilter.PageSize);

            return result.ToList();
        }

        public long GetAllFilteredCount(OrganizationFilter organizationFilter)
        {
            return GetOrganizationsFiltered(organizationFilter).Count();
        }

        public IQueryable<Organization> FilterByName(string name)
        {
            return context.Organization.Where(x => !x.IsDeleted && x.Name.ToLower().Contains(name.ToLower()));
        }

        public long GetAllCount()
        {
            return context.Organization.Where(x => !x.IsDeleted).Count();
        }

        public long GetAllEntriesCountByCountry(int? countryId)
        {
            return context.Organization.Include(x => x.Address).Where(x => x.Address != null && x.Address.CountryId == countryId).Count();
        }


        public Organization GetById(int id, bool noTracking = false)
        {
            var data = context.Organization.Where(x => x.OrganizationId == id)
                .Include(x => x.Address)
                .Include(x => x.Telecoms)
                .Include("OrganizationRelation")
                .Include("OrganizationRelation.Child")
                .Include("OrganizationRelation.Parent");

            if (noTracking)
            {
                data = data.AsNoTracking();
            }

            return data.FirstOrDefault();
        }

        public Organization GetByName(string name)
        {
            return context.Organization.FirstOrDefault(x => !x.IsDeleted && x.Name.Equals(name));
        }

        public List<DocumentClinicalDomain> GetClinicalDomainsForIds(List<int> ids)
        {
            return context.OrganizationClinicalDomain
                .Where(x => ids.Contains(x.OrganizationId))
                .Select(x => x.ClinicalDomainId)
                .Distinct()
                .ToList()
                .Select(x => (DocumentClinicalDomain)x)
                .ToList();
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, Dictionary<int, string> countries)
        {
            List<OrganizationUsersCount> result = new List<OrganizationUsersCount>();
            result = context.Organization
                .Include(x => x.OrganizationRelation)
                .Include(x => x.Address)
                .Where(x => !x.IsDeleted)
                .Select(organization => new OrganizationUsersCount()
                {
                    OrganizationName = organization.Name,
                    UsersCount = organization.NumOfUsers,
                    PartOf = organization.OrganizationRelation != null ? organization.OrganizationRelation.ParentId : 0,
                    OrganizationId = organization.OrganizationId,
                    CountryId = organization.Address != null ? organization.Address.CountryId : null
                }) 
                .ToList();

            var ancestor = result.Where(x => x.PartOf == 0);

            SetOrganizationsChildren(result);

            return ancestor
                .Where(x => string.IsNullOrWhiteSpace(term) || x.FoundName(term))
                .Where(x => countries == null || countries.Count == 0 || x.FoundCountry(countries.Keys.ToList()))
                .ToList();
        }

        public void InsertOrUpdate(Organization organization)
        {
            if (organization.OrganizationId == 0)
            {
                context.Organization.Add(organization);
            }
            else
            {
                context.Entry(organization).OriginalValues["RowVersion"] = organization.RowVersion;
                context.Entry(organization).State = EntityState.Modified;
                organization.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public List<UserOrganization> GetUsersByOrganizationIds(List<int> ids)
        {
            return context.User
                .SelectMany(x => x.Organizations)
                .Where(x => ids.Contains(x.OrganizationId))
                .Include(x => x.User)
                .Include(x => x.User.Roles)
                .Include("User.Roles.Role")
                .Include(x => x.Organization)
                .ToList();
        }

        private void SetOrganizationsChildren(List<OrganizationUsersCount> allOrganization)
        {
            foreach (OrganizationUsersCount organization in allOrganization)
            {
                organization.Children = SetOrganizationChildren(organization.OrganizationId, allOrganization);
            }
        }
        public List<Organization> GetByIds(List<int> ids)
        {
            return context.Organization.Where(x => !x.IsDeleted && ids.Contains(x.OrganizationId)).ToList();
        }

        public void InsertOrganizationRelation(OrganizationRelation relation)
        {

            if (relation.OrganizationRelationId == 0)
            {
                context.OrganizationRelation.Add(relation);
                context.SaveChanges();
            }
        }

        private List<OrganizationUsersCount> SetOrganizationChildren(int organizationId, List<OrganizationUsersCount> allOrganizations)
        {
            List<OrganizationUsersCount> allChildren = new List<OrganizationUsersCount>();
            foreach (OrganizationUsersCount organization in allOrganizations.Where(x => x.PartOf != 0 && x.PartOf == organizationId))
            {
                if (organizationId == organization.PartOf)
                {
                    allChildren.Add(organization);
                }
            }
            return allChildren;
        }

        private IQueryable<Organization> GetOrganizationsFiltered(OrganizationFilter filterData)
        {
            IQueryable<Organization> query = context.Organization
                .Include(x => x.Identifiers)
                .Where(org => (filterData.Name == null || org.Name.ToLower().Contains(filterData.Name.ToLower()))
              && !org.IsDeleted
              && (filterData.Alias == null || org.Alias.Equals(filterData.Alias))
              && (filterData.City == null || org.Address.City.Equals(filterData.City))
              && (filterData.State == null || org.Address.State.Equals(filterData.State))
              && (filterData.PostalCode == null || org.Address.PostalCode.Equals(filterData.PostalCode))

              && (filterData.Street == null || org.Address.Street.Equals(filterData.Street))
              && (filterData.CountryId == null || org.Address.CountryId == filterData.CountryId)
              && (filterData.Type == null || org.TypesString.Contains(filterData.Type))
                );

            if (!string.IsNullOrWhiteSpace(filterData.ClinicalDomain))
            {
                query = query
                   .Join(
                      context.OrganizationClinicalDomain,
                      org => org.OrganizationId,
                      cD => cD.OrganizationId,
                      (org, cD) => new { org, cD })
                   .Where(a => a.cD.ClinicalDomain.Name.Equals(filterData.ClinicalDomain))
                   .Select(a => a.org);
            }

            if (filterData.Parent != null)
            {
                query = query.Where(org => org.OrganizationRelation.Parent.OrganizationId == filterData.Parent);
            }

            query = FilterByIdentifier(query, filterData.IdentifierType, filterData.IdentifierValue);

            return query;
        }

        private IQueryable<Organization> FilterByIdentifier(IQueryable<Organization> query, string system, string value)
        {
            IQueryable<Organization> result = query;
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(value))
            {
                if (system.Equals(ResourceTypes.O4PatientId))
                {
                    if (Int32.TryParse(value, out int O4PatientId))
                    {
                        result = query.Where(x => x.OrganizationId.Equals(O4PatientId));
                    }
                }
                else
                {
                    result = query
                        .Where(x => x.Identifiers.Any(y => !string.IsNullOrEmpty(y.System) && !string.IsNullOrEmpty(y.Value) && y.System.Equals(system) && y.Value.Equals(value)));
                }
            }
          
            return result;
        }

        private void DoDeleteCheck(Organization organization)
        {
            if (organization.NumOfUsers > 0)
            {
                throw new UserAdministrationException(System.Net.HttpStatusCode.Conflict, $"Cannot delete because there are active users in {organization.Name}");
            }
        }

        private IQueryable<Organization> SortByField(IQueryable<Organization> result, OrganizationFilter organizationFilter) 
        {
            switch (organizationFilter.ColumnName)
            {
                case AttributeNames.Address:
                    if (organizationFilter.IsAscending)
                        return result.OrderBy(x => x.Address.City)
                                .ThenBy(x => x.Address.PostalCode)
                                .ThenBy(x => x.Address.Country)
                                .Skip((organizationFilter.Page - 1) * organizationFilter.PageSize)
                                .Take(organizationFilter.PageSize);
                    else
                        return result.OrderByDescending(x => x.Address.City)
                                .ThenByDescending(x => x.Address.PostalCode)
                                .ThenByDescending(x => x.Address.Country)
                                .Skip((organizationFilter.Page - 1) * organizationFilter.PageSize)
                                .Take(organizationFilter.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, organizationFilter.ColumnName, organizationFilter.IsAscending)
                                 .Skip((organizationFilter.Page - 1) * organizationFilter.PageSize)
                                 .Take(organizationFilter.PageSize);
            }
        }
    }
}
