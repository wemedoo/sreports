using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Organization;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace sReportsV2.BusinessLayer.Implementations
{
    public class OrganizationBLL : IOrganizationBLL
    {

        private readonly IOrganizationDAL organizationDAL;
        private readonly IOrganizationRelationDAL organizationRelationDAL;
        private readonly IAddressDAL addressDAL;

        public PaginationDataOut<OrganizationDataOut, DataIn> ReloadTable(OrganizationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            IQueryable<Organization> organizationsFiltered = organizationDAL.Filter(Mapper.Map<OrganizationFilter>(dataIn));
            PaginationDataOut<OrganizationDataOut, DataIn> result = new PaginationDataOut<OrganizationDataOut, DataIn>()
            {
                Count = (int)organizationsFiltered.Count(),
                Data = Mapper.Map<List<OrganizationDataOut>>(organizationsFiltered.OrderByDescending(x => x.Id).Skip((dataIn.Page - 1)*dataIn.PageSize).Take(dataIn.PageSize).ToList()),
                DataIn = dataIn
            };

            return result;
        }

        public OrganizationBLL(IOrganizationDAL organizationDAL, IOrganizationRelationDAL organizationRelationDAL, IAddressDAL addressDAL)
        {
            this.organizationDAL = organizationDAL;
            this.organizationRelationDAL = organizationRelationDAL;
            this.addressDAL = addressDAL;
        }

        public void Delete(OrganizationDataIn organizationDataIn)
        {
            Organization organization = Mapper.Map<Organization>(organizationDataIn);
            organizationDAL.Delete(organization);
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
            organizationDataOuts = filtered.OrderBy(x => x.Id).Skip(dataIn.Page * 10).Take(10)
                .Select(x => new AutocompleteDataOut()
                {
                    id = x.Id.ToString(),
                    text = x.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 10.00) > dataIn.Page,
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
            Organization organization = Mapper.Map<Organization>(organizationDataIn);
            if (organization.Id != 0)
            {
                Organization dbOrganization = organizationDAL.GetById(organization.Id);
                if (dbOrganization == null) throw new ObjectNotFoundException();

                dbOrganization.CopyData(organization);
                organization = dbOrganization;
            }
            organizationDAL.InsertOrUpdate(organization);

            organization.SetRelation(organizationDataIn.ParentId);
            organization.SetClinicalDomains(organizationDataIn.ClinicalDomain);
            organizationDAL.InsertOrUpdate(organization);
        }

        private void LoadHierarchyTree(List<OrganizationRelation> hierarchyList, int parentId, List<OrganizationDataOut> hierarchy)
        {
            OrganizationRelation parentHierarchy = hierarchyList.FirstOrDefault(x => x.ChildId == parentId);
            if (parentHierarchy != null)
            {
                var parent = Mapper.Map<OrganizationDataOut>(parentHierarchy.Parent);
                var child = Mapper.Map<OrganizationDataOut>(parentHierarchy.Child);
                
                if(!hierarchy.Any(x => x.Id == parent.Id))
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

        public long GetAllCount()
        {
            return organizationDAL.GetAllCount();
        }

        public long GetAllEntriesCountByCountry(string country)
        {
            return organizationDAL.GetAllEntriesCountByCountry(country);
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries)
        {
            return organizationDAL.GetOrganizationUsersCount(term, countries);
        }
    }
}
