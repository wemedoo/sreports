using sReportsV2.Common.Enums.DocumentPropertiesEnums;
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
        public DocumentClass Class { get; set; }
        public DocumentPurposeDataOut Purpose { get; set; }
        public DocumentScopeOfValidityDataOut ScopeOfValidity { get; set; }
        public  List<DocumentClinicalDomain?> ClinicalDomain { get; set; }
        public DocumentClinicalContext ClinicalContext { get; set; }
        public AdministrativeContext? AdministrativeContext { get; set; }
        public string Description { get; set; }
        public string ClinicalDomainToString()
        {
            return string.Join(",", this.ClinicalDomain);
        }
    }
}