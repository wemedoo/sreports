using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DiagnosticReport.DataOut
{
    public class DiagnosticReportCreateFromPatientDataOut
    {
        public EncounterDataOut Encounter { get; set; }
        public List<FormInstanceDataOut> FormInstances { get; set; }
        public FormDataOut CurrentForm { get; set; }
    }
}