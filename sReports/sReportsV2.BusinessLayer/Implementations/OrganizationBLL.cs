using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Organization;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.Common.Extensions;
using sReportsV2.SqlDomain.Filter;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Autocomplete;
using System.Data.Entity.Core;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Common.Constants;
using System.Data.Entity.Infrastructure;
using sReportsV2.Common.Exceptions;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class OrganizationBLL : IOrganizationBLL
    {

        private readonly IOrganizationDAL organizationDAL;
        private readonly IOrganizationRelationDAL organizationRelationDAL;

        public OrganizationBLL(IOrganizationDAL organizationDAL, IOrganizationRelationDAL organizationRelationDAL)
        {
            this.organizationDAL = organizationDAL;
            this.organizationRelationDAL = organizationRelationDAL;
        }

        public PaginationDataOut<OrganizationDataOut, DataIn> ReloadTable(OrganizationFilterDataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            OrganizationFilter filterData = Mapper.Map<OrganizationFilter>(dataIn);

            List<Organization> organizationsFiltered = organizationDAL.GetAll(filterData);
            PaginationDataOut<OrganizationDataOut, DataIn> result = new PaginationDataOut<OrganizationDataOut, DataIn>()
            {
                Count = (int) organizationDAL.GetAllFilteredCount(filterData),
                Data = Mapper.Map<List<OrganizationDataOut>>(organizationsFiltered),
                DataIn = dataIn
            };

            return result;
        }

        public void Delete(OrganizationDataIn organizationDataIn)
        {
            try
            {
                Organization organization = Mapper.Map<Organization>(organizationDataIn);
                organizationDAL.Delete(organization);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public bool ExistIdentifier(IdentifierDataIn dataIn)
        {
            Identifier identifier = Mapper.Map<Identifier>(dataIn);
            bool result = organizationDAL.ExistIdentifier(identifier);
            return result;
        }

        public AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            List<AutocompleteDataOut> organizationDataOuts = new List<AutocompleteDataOut>();
            IQueryable<Organization> filtered = organizationDAL.FilterByName(dataIn.Term);
            organizationDataOuts = filtered.OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.OrganizationId.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = organizationDataOuts
            };

            return result;
        }

        public OrganizationDataOut GetOrganizationById(int organizationId)
        {
            return Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(organizationId));
        }

        public OrganizationDataOut GetOrganizationForEdit(int organizationId)
        {
            Organization organization = organizationDAL.GetById(organizationId);
            return Mapper.Map<OrganizationDataOut>(organization);
        }

        public List<OrganizationDataOut> ReloadHierarchy(int? parentId)
        {
            List<OrganizationDataOut> result = new List<OrganizationDataOut>();
            List<OrganizationRelation> hierarchyList = organizationRelationDAL.GetOrganizationHierarchies();
            LoadHierarchyTree(hierarchyList, parentId.GetValueOrDefault(), result);
            return result;
        }

        public void Insert(OrganizationDataIn organizationDataIn)
        {
            try
            {
                Organization organization = Mapper.Map<Organization>(organizationDataIn);
                if (organization.OrganizationId != 0)
                {
                    Organization dbOrganization = organizationDAL.GetById(organization.OrganizationId);
                    if (dbOrganization == null) throw new ObjectNotFoundException();

                    dbOrganization.CopyData(organization);
                    organization = dbOrganization;
                }
                organizationDAL.InsertOrUpdate(organization);

                organization.SetRelation(organizationDataIn.ParentId);
                organization.SetClinicalDomains(organizationDataIn.ClinicalDomain);
                organizationDAL.InsertOrUpdate(organization);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, Dictionary<int, string> countries)
        {
            return organizationDAL.GetOrganizationUsersCount(term, countries);
        }

        public List<OrganizationUsersDataOut> GetUsersByOrganizationsIds(List<int> ids)
        {
            List<UserOrganization> userOrganizations = GetUsersByOrganizationsWithPermission(organizationDAL.GetUsersByOrganizationIds(ids));
            
            return userOrganizations.GroupBy(x => new { x.OrganizationId, x.Organization.Name }).Select(x => new OrganizationUsersDataOut
            {
                Id = x.Key.OrganizationId,
                Name = x.Key.Name,
                Users = Mapper.Map<List<UserDataOut>>(x.Select(y => y.User).ToList())
            }).ToList();
        }

        private void LoadHierarchyTree(List<OrganizationRelation> hierarchyList, int parentId, List<OrganizationDataOut> hierarchy)
        {
            OrganizationRelation parentHierarchy = hierarchyList.FirstOrDefault(x => x.ChildId == parentId);
            if (parentHierarchy != null)
            {
                var parent = Mapper.Map<OrganizationDataOut>(parentHierarchy.Parent);
                var child = Mapper.Map<OrganizationDataOut>(parentHierarchy.Child);

                if (!hierarchy.Any(x => x.Id == parent.Id))
                {
                    hierarchy.Add(parent);
                }

                if (!hierarchy.Any(x => x.Id == child.Id))
                {
                    hierarchy.Add(child);
                }

                LoadHierarchyTree(hierarchyList, parentHierarchy.ParentId, hierarchy);
            }
            else
            {
                Organization organization = organizationDAL.GetById(parentId);
                if (organization != null)
                {
                    var org = Mapper.Map<OrganizationDataOut>(organization);
                    if (!hierarchy.Any(x => x.Id == org.Id))
                    {
                        hierarchy.Add(org);
                    }
                }
            }
        }

        private List<UserOrganization> GetUsersByOrganizationsWithPermission(List<UserOrganization> userOrganizations)
        {
            List<UserOrganization> userOrganizationsWithPermission = new List<UserOrganization>();

            foreach (UserOrganization userOrganization in userOrganizations)
            {
                if (userOrganization.User.UserHasPermission(PermissionNames.FindConsensus, ModuleNames.Designer))
                {
                    userOrganizationsWithPermission.Add(userOrganization);
                }
            };

            return userOrganizationsWithPermission;
        }
    }
}
