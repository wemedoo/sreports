using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.DTOs.DTOs.Field.DataIn.Custom;
using System.Collections.Generic;

namespace sReportsV2.DTOs.FormInstance
{
    public class FormInstanceFilterDataIn : Common.DataIn
    {
        public string FormId { get; set; }
        public string VersionId { get; set; }

        public string FormInstanceId { get; set; }

        public string Title { get; set; }

        public int ThesaurusId { get; set; }

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

        public string Content { get; set; }
        public List<int> UserIds { get; set; } = new List<int> { };
        public List<int> PatientIds { get; set; } = new List<int> { };
        public List<CustomFieldFilterDataIn> CustomFieldFiltersDataIn { get; set; } = new List<CustomFieldFilterDataIn> { };
        public string FieldFiltersOverallOperator { get; set; }
    }
}