using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceFilterData
    {
        public string FormId { get; set; }

        public string FormInstanceId { get; set; }

        public string Title { get; set; }

        public int ThesaurusId { get; set; }

        public string VersionId { get; set; }


        public int PageSize { get; set; }

        public int Page { get; set; }

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

        public string Content { get; set; }
        public List<int> UserIds { get; set; } = new List<int> { }; 
        public List<int> PatientIds { get; set; } = new List<int> { };
        public List<CustomFieldFilterData> CustomFieldFiltersData { get; set; } = new List<CustomFieldFilterData> { };
        public string FieldFiltersOverallOperator { get; set; }
    }
}
