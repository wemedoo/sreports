using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DataIn
{
    public class LinkOrganizationDataIn
    {
        public string OrganizationName { get; set; }
        public List<int> OrganizationsIds{ get; set; }
    }
}