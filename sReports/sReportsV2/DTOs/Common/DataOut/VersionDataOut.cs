using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class VersionDataOut
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public UserDataDataOut User { get; set; }
        public VersionType Type { get; set; }
        public OrganizationDataOut Organization { get; set; }
    }
}