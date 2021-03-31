using sReportsV2.Domain.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IOrganizationService 
    {
        void Insert(Organization organization);
        List<Organization> GetAll(int pageSize, int page);
        long GetAllEntriesCount();
        Organization GetOrganizationById(string id);
        bool Delete(string organizationId, DateTime lastUpdate);
        List<Organization> GetOrganizationByOrganizationId(string id);
        List<Organization> GetOrganizationsByIds(List<string> ids);
        List<Organization> GetByParameters(OrganizationFilter organizationFilter);
        List<Organization> SearchByName(string name, int page, int pageSize);
        bool ExistsOrganizationByIdentifier(IdentifierEntity identifier);
        bool ExistsOrganizationById(string id);
        long GetSearchByNameCount(string name);
        List<OrganizationUsersCount> GetOrganizationUsersCount();
        List<Organization> GetOrganizations();
    }
}
