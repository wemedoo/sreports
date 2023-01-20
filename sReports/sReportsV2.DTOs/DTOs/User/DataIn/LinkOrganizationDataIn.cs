using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DataIn
{
    public class LinkOrganizationDataIn
    {
        public int OrganizationId { get; set; }
        public List<int> OrganizationsIds{ get; set; }
    }
}