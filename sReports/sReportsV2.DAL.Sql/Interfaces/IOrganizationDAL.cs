using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface IOrganizationDAL
    {
        List<DocumentClinicalDomain> GetClinicalDomainsForIds(List<int> ids);
        Organization GetByName(string name);
        Organization GetById(int id, bool noTracking = false);
        void InsertOrUpdate(Organization organization);
        IQueryable<Organization> Filter(OrganizationFilter organizationFilter);
        void Delete(Organization organization);
        bool ExistIdentifier(Identifier identifier);
        IQueryable<Organization> FilterByName(string name);
        long GetAllCount();
        long GetAllEntriesCountByCountry(string country);
        List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries);
        List<Organization> GetByIds(List<int> ids);
        void InsertOrganizationRelation(OrganizationRelation relation);

    }
}
