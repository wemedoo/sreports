using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;

namespace sReportsV2.Domain.Entities.Form
{
    public class FormFilterData
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public FormDefinitionState? State { get; set; }
        public int OrganizationId { get; set; }

        public string ActiveLanguage { get; set; }

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

        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }
        public List<string> FormStates { get; set; } = new List<string>();

    }
}
