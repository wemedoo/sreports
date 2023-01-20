using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Patient.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterDataOut
    {
        public int Id { get; set; }
        public int EpisodeOfCareId { get; set; }
        public string PatientId { get; set; }
        public CustomEnumDataOut Status { get; set; }
        public CustomEnumDataOut Class { get; set; }
        public CustomEnumDataOut Type { get; set; }
        public CustomEnumDataOut ServiceType { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<PatientFormInstanceDataOut> FormInstances { get; set; }
        public PeriodDTO Period { get; set; }

    }
}