using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataOut
{
    public class OrganizationUsersCountDataOut
    {
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int UsersCount { get; set; }
        public List<OrganizationUsersCountDataOut> Children { get; set; }
    }
}