using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DataIn
{
    public class UserDataDataIn
    {
        public string Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public List<string> Organizations { get; set; }
        public List<string> Roles { get; set; }
    }
}