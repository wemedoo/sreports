using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DocumentProperties.DataOut
{
    public class DocumentPropertiesDataOut
    {
        public DocumentClassEnum Class { get; set; }
        public DocumentPurposeDataOut Purpose { get; set; }
        public DocumentScopeOfValidityDataOut ScopeOfValidity { get; set; }
        public DocumentClinicalDomain ClinicalDomain { get; set; }
        public DocumentClinicalContext ClinicalContext { get; set; }
        public AdministrativeContext AdministrativeContext { get; set; }
        public string Description { get; set; }
        public List<VersionDataOut> VersionHistory { get; set; }
    }
}