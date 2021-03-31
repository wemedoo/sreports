using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.Organization;
using sReportsV2.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Common
{
    public class VersionViewModel
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public UserDataViewModel User {get;set;}
        public VersionType Type { get; set; }
        public OrganizationDataOut Organization { get; set; }

    }
}