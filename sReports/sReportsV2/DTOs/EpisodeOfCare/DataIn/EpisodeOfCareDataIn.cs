using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareDataIn
    {
        public string Description { get; set; }
        public string PatientId { get; set; }
        public List<EpisodeOfCareStatus> ListHistoryStatus { get; set; }
        public string Id { get; set; }
        [Required]
        public string Status { get; set; }
        public string Type { get; set; }
        public string DiagnosisCondition { get; set; }
        [Required]
        public string DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
        [Required]
        public PeriodDTO Period { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}