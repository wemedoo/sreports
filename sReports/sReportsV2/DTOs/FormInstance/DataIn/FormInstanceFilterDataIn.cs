using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormInstance
{
    public class FormInstanceFilterDataIn : Common.DataIn
    {
        public string FormId { get; set; }
        public string VersionId { get; set; }

        public string FormInstanceId { get; set; }

        public string Title { get; set; }

        public string ThesaurusId { get; set; }

        public DocumentClassEnum? DocumentClass { get; set; }

        public string ClassesOtherValue { get; set; }

        public DocumentGeneralPurposeEnum? GeneralPurpose { get; set; }

        public ContextDependent? ContextDependent { get; set; }

        public DocumentExplicitPurpose? ExplicitPurpose { get; set; }

        public DocumentScopeOfValidityEnum? ScopeOfValidity { get; set; }

        public DocumentClinicalDomain? ClinicalDomain { get; set; }

        public DocumentClinicalContextEnum? ClinicalContext { get; set; }

        public FollowUp? FollowUp { get; set; }

        public AdministrativeContext? AdministrativeContext { get; set; }

        public bool IsSimplifiedLayout { get; set; }
        public string Language { get; set; }
    }
}