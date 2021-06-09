using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DTO
{
    public class ClinicalTrialDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string SponosorId { get; set; }
        public string WemedooId { get; set; }
        public bool? IsArchived { get; set; }
        public ClinicalTrialRecruitmentsStatus? Status { get; set; }
        public ClinicalTrialRole? Role { get; set; }
        public int UserId { get; set; }

    }
}