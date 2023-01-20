using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterDataIn
    {
        public string EpisodeOfCareId { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public int Class { get; set; }
        public int Type { get; set; }
        public int ServiceType { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string PatientId { get; set; }
        public PeriodDTO Period { get; set; }
    }
}