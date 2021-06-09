using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormDefinitionState
    {
        DesignPending,
        Design,
        ReviewPending,
        Review,
        ReadyForDataCapture,
        Archive
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormFieldDependableType
    {
        Toggle,
        Email
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayoutType
    {
        Horizontal,
        Vertical
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        Male,
        Female,
        Other,
        Unknown
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EOCStatus
    {
        Planned,
        Waitlist,
        Active,
        Onhold,
        Finished,
        Cancelled,
        EnteredInError
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TelecomSystemType
    {
        Phone,
        Fax,
        Email,
        Pager,
        Url,
        Sms,
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TelecomUseType
    {
        Home,
        Work,
        Temp,
        Old,
        Mobile
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierUseType
    {
        Usual,
        Official,
        Temp,
        Secondary,
        Old,
        
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfIdentifier
    {
        IdentifierName,
        IdentifierValue,
        IdentifierUse
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierKind
    {
        PatientIdentifierType,
        OrganizationIdentifierType
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormState
    {
        Finished,
        OnGoing
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredefinedType
    {
        DiagnosisRole,
        EncounterClassification,
        EncounterStatus,
        EncounterType,
        EpisodeOfCareType,
        OrganizationType,
        ServiceType,
        PatientIdentifierType,
        OrganizationIdentifierType
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ThesaurusState
    {
        Draft,
        Production,
        Deprecated,
        Disabled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ThesaurusMergeState
    {
        Pending,
        Completed,
        NotComplited
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SimilarTermType
    {
        O4MTId,
        UMLS
        
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstitutionalLegalForm
    {
        PrivatePractice,
        StateUniversityHospital,
        CompanyWithLimitedLiabilities,
        PrivateCompany,
        PublicCompany
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstitutionalOrganizationalForm
    {
        Institute,
        Center,
        Clinic
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserPrefix
    {
        Mr,
        Ms
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AcademicPosition
    {
        Professor,
        AssistantProfesssor,
        DoctorOfPhilosophy,
        Privatdozent
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClinicalTrialRecruitmentsStatus
    {
        NotYetRecruiting,
        Recruiting,
        EnrollingByInvitation,
        NotRecruiting,
        Suspended,
        Terminated,
        Completed,
        Withdrawn,
        Unknown,
        Active
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserState
    {
        Active,
        Archived,
        Blocked
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ClinicalTrialRole
    {
        PrincipalInvestigator,
        CoInvestigator,
        StudyNurse,
        ClinicalResearchAdministrator,
        MedicalMonitor,
        ClinicalTrialPharmacist
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommentState
    {
        Created,
        Archive,
        Resolve,
        Reject
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormTypeField
    {
        FormChapter,
        FormPage,
        FormFieldSet,
        Field,
        FieldRadioOrCheckButton
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormItemLevel
    {
        Form,
        Chapter,
        Page,
        FieldSet,
        Field,
        FieldValue
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConsensusFindingState
    {
        OnGoing,
        InIteration,
        Finished
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IterationState
    {
        NotStarted,
        InProgress,
        Finished
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VersionType
    {
        MAJOR,
        MINOR,
        PATCH
    }

    public enum CustomEnumType
    {
        ServiceType,
        OrganizationType,
        EpisodeOfCareType,
        EncounterType,
        EncounterStatus,
        EncounterClassification,
        DiagnosisRole,
        PatientIdentifierType,
        OrganizationIdentifierType
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GlobalUserSource
    {
        Microsoft,
        Google,
        Internal
    }
}
