using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataOut
{
    public class OrganizationUsersDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserDataOut> Users { get; set; }
    }
}