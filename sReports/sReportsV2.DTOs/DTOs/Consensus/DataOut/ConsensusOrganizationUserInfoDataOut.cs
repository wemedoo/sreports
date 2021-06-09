using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class ConsensusOrganizationUserInfoDataOut
    {
        public int UsersCount { get; set; }
        public int OrganizationsCount { get; set; }
        public int OrganizationsCountByState { get; set; }
    }
}