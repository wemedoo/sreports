using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.SqlDomain.Filter;
using System.Data.Entity;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;

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
            Organization fromDb = context.Organization.FirstOrDefault(x => x.Id == organization.Id);
            if(fromDb != null)
            {
                fromDb.RowVersion = organization.RowVersion;
                fromDb.IsDeleted = true;
            }
            context.SaveChanges();
        }

        public bool ExistIdentifier(Identifier identifier)
        {
            return context.Identifiers.Any(x => x.System.Equals(identifier.System) && x.Value.Equals(identifier.Value));
        }

        public IQueryable<Organization> Filter(OrganizationFilter organizationFilter)
        {
            return context.Organization.Where(x =>  (organizationFilter.Name == null || x.Name.Equals(organizationFilter.Name))
                                                     && !x.IsDeleted);
        }

        public IQueryable<Organization> FilterByName(string name)
        {
            return context.Organization.Where(x => x.Name.ToLower().Contains(name.ToLower()));
        }

        public long GetAllCount()
        {
            return context.Organization.Where(x => !x.IsDeleted).Count();
        }

        public long GetAllEntriesCountByCountry(string country)
        {
            return context.Organization.Include(x => x.Address).Where(x => x.Address != null && x.Address.Country == country).Count();
        }


        public Organization GetById(int id, bool noTracking = false)
        {
            var data = context.Organization.Where(x => x.Id == id)
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

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries)
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
                    OrganizationId = organization.Id,
                    Country = organization.Address != null ? organization.Address.Country : string.Empty
                }) 
                .ToList();


            var testS = context.Organization
                .Include(x => x.OrganizationRelation)
                .Include(x => x.Address)
                .Where(x => !x.IsDeleted && x.OrganizationRelation != null).ToList();

            var ancestor = result.Where(x => x.PartOf == 0);

            SetOrganizationsChildren(result);

            return ancestor
                .Where(x => string.IsNullOrWhiteSpace(term) || x.FoundName(term))
                .Where(x => countries == null || countries.Count == 0 || x.FoundCountry(countries))
                .ToList();
        }

        public void InsertOrUpdate(Organization organization)
        {
            if (organization.Id == 0)
            {
                context.Organization.Add(organization);
                organization.EntryDatetime = DateTime.Now;
            }
            else
            {
                context.Entry(organization).State = EntityState.Modified;
            }

            organization.LastUpdate = DateTime.Now;

            context.SaveChanges();
        }

        private void SetOrganizationsChildren(List<OrganizationUsersCount> allOrganization)
        {
            foreach (OrganizationUsersCount organization in allOrganization)
            {
                organization.Children = SetOrganizationChildren(organization.OrganizationId, allOrganization);
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

        public List<Organization> GetByIds(List<int> ids)
        {
            return context.Organization.Where(x => !x.IsDeleted && ids.Contains(x.Id)).ToList();
        }

        public void InsertOrganizationRelation(OrganizationRelation relation)
        {

            if (relation.Id == 0) 
            {
                relation.EntryDatetime = DateTime.Now;
                context.OrganizationRelation.Add(relation);
                context.SaveChanges();
            }
        }
    }
}
