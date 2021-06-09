using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Patient.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormInstance.DataOut
{
    public class FormInstanceDataOut
    {
        public string Id { get; set; }
        public PatientTableDataOut Patient { get; set; }
        public FormAboutDataOut About { get; set; }
        public string FormDefinitionId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime LastUpdate { get; set; }
        public UserDataOut User { get; set; }
        public OrganizationDataOut Organization { get; set; }
        public string Language { get; set; }
        public string ThesaurusId { get; set; }
        public List<FormChapterDataOut> Chapters { get; set; } = new List<FormChapterDataOut>();
        public DocumentPropertiesDataOut DocumentProperties { get; set; }
        public List<FormInstanceReferralDataOut> Referrals {get; set; }

    }
}