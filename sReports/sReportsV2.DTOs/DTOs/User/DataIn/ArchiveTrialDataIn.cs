using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DataIn
{
    public class ArchiveTrialDataIn
    {
        public DateTime? LastUpdate { get; set; }
        public int UserId { get; set; }
        public int ClinicalTrialId { get; set; }
    }
}