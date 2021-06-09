using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;

namespace sReportsV2.DTOs.Form
{
    public class FormFilterDataIn : Common.DataIn
    {
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public FormDefinitionState? State { get; set; }
        public DocumentClassEnum? Classes { get; set; }

        public string ClassesOtherValue { get; set; }

        public DocumentGeneralPurposeEnum? GeneralPurpose { get; set; }

        public ContextDependent? ContextDependent { get; set; }

        public DocumentExplicitPurpose? ExplicitPurpose { get; set; }

        public DocumentScopeOfValidityEnum? ScopeOfValidity { get; set; }

        public DocumentClinicalDomain? ClinicalDomain { get; set; }

        public DocumentClinicalContextEnum? ClinicalContext { get; set; }

        public FollowUp? FollowUp { get; set; }

        public AdministrativeContext? AdministrativeContext { get; set; }

        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }

    }
}