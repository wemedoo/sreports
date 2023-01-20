using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareDataOut
    {
        public string Description { get; set; }
        public List<EpisodeOfCareStatus> ListHistoryStatus { get; set; }
        public List<GuidelineInstanceDataOut> ListGuidelines { get; set; } = new List<GuidelineInstanceDataOut>();
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string OrganizationRef { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public string DiagnosisCondition { get; set; }
        public CustomEnumDataOut DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
        public PeriodDTO Period { get; set; }
        public List<DiagnosticReportDataOut> DiagnosticReports { get; set; }
        public PatientDataOut Patient { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<EncounterDataOut> Encounters;

    }
}