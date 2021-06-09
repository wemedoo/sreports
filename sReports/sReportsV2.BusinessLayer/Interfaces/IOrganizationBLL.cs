using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IOrganizationBLL
    {
        PaginationDataOut<OrganizationDataOut, DataIn> ReloadTable(OrganizationFilterDataIn dataIn);
        OrganizationDataOut GetOrganizationById(int organizationId);
        void Insert(OrganizationDataIn organization);
        void Delete(OrganizationDataIn organization);
        OrganizationDataOut GetOrganizationForEdit(int organizationId);
        bool ExistIdentifier(IdentifierDataIn dataIn);
        AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn autocompleteDataIn);
        List<OrganizationDataOut> ReloadHierarchy(int? parent);
        long GetAllCount();
        long GetAllEntriesCountByCountry(string country);
        List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries);


    }
}
