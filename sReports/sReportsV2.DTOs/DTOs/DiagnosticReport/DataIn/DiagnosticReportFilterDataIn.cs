using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DiagnosticReport
{
    public class DiagnosticReportFilterDataIn : DataIn
    {
        public string EpisodeOfCareId { get; set; }
    }
}