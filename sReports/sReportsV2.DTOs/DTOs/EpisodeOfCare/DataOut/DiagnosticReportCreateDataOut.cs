using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DiagnosticReport;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class DiagnosticReportCreateDataOut
    {
        public EpisodeOfCareDataOut EpisodeOfCare { get; set; }

        public PatientDataOut Patient { get; set; }

        public FormDataOut Form { get; set; }

        public List<string> Referrals { get; set; }

    }
}