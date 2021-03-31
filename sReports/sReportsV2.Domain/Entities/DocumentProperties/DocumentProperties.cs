using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    public class DocumentProperties
    {
        public DocumentClassEnum Class { get; set; }
        public DocumentPurpose Purpose { get; set; }
        public DocumentScopeOfValidity ScopeOfValidity { get; set; }
        public DocumentClinicalDomain? ClinicalDomain { get; set; }
        public DocumentClinicalContext ClinicalContext { get; set; }
        public AdministrativeContext AdministrativeContext { get; set; }
        public string Description { get; set; }
        public List<Domain.Entities.Common.Version> VersionHistory { get; set; }
    }
}
