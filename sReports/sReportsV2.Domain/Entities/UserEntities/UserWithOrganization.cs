using sReportsV2.Domain.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.UserEntities
{
    public class UserWithOrganization
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveLanguage { get; set; }
        public List<Organization> Organizations { get; set; }
    }
}
