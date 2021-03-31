using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Common;
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
        public string EpisodeOfCareId { get; set; }
        public string Id { get; set; }
        public EnumDataOut Status { get; set; }
        public EnumDataOut Class { get; set; }
        public EnumDataOut Type { get; set; }
        public ServiceTypeDTO ServiceType { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<PatientFormInstanceDataOut> FormInstances { get; set; }

    }
}