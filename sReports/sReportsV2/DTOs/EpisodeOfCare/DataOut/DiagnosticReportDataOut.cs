using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class DiagnosticReportDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThesaurusId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public UserDataDataOut User { get; set; }
        public OrganizationDataOut Organization{get;set;}
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; } 

    }
}