using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class UserDataDataOut
    {
        public string Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public List<OrganizationDataOut> Organizations { get; set; }
        public List<string> Roles { get; set; }
        public override string ToString()
        {
            return $"{Username} ({FirstName} {LastName})";
        }
    }
}