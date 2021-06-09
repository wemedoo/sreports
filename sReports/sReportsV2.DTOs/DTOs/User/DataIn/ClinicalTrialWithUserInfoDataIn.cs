using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DTO
{
    public class ClinicalTrialWithUserInfoDataIn
    {
        public ClinicalTrialDTO ClinicalTrial { get; set; }
        public int? UserId { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}