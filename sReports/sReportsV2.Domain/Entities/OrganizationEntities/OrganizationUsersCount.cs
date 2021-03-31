using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.OrganizationEntities
{
    public class OrganizationUsersCount
    {
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int UsersCount { get; set; }
        public List<OrganizationUsersCount> Children { get; set; }
        public string PartOf { get; set; }
    }
}
