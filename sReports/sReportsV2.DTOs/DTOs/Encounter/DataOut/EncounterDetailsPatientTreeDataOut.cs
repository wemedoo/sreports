using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter.DataOut
{
    public class EncounterDetailsPatientTreeDataOut
    {
        public EncounterDataOut Encounter { get; set; }
        public List<FormInstanceDataOut> FormInstances { get; set; }
        public List<FormDataOut> Forms { get; set; }
    }
}