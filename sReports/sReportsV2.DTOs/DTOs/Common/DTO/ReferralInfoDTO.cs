using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class ReferralInfoDTO
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string Title { get; set; }
        public string ThesaurusId { get; set; }
        public UserDataOut User { get; set; }
        public DateTime? LastUpdate { get; set; }
        public OrganizationDataOut Organization { get; set; }
        public List<KeyValueDTO> ReferrableFields { get; set; }
    }
}