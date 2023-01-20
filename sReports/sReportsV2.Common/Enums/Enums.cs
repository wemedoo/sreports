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
        DesignPending = 0,
        Design = 1,
        ReviewPending = 2,
        Review = 3,
        ReadyForDataCapture = 4,
        Archive = 5
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
        Male = 0,
        Female = 1,
        Other = 2,
        Unknown = 3
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
        OnGoing,
        Signed
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredefinedType
    {
        DiagnosisRole,
        EncounterClassification,
        EncounterStatus,
        EncounterType,
        EpisodeOfCareType,
        PatientIdentifierType,
        ServiceType,
        OrganizationType,
        OrganizationIdentifierType,
        CodeSystem
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ThesaurusState
    {
        Draft = 0,
        Production = 1,
        Deprecated = 2,
        Disabled = 3,
        Curated = 4, 
        Uncurated = 5,
        MetadataIncomplete = 6,
        RequiresDiscussion = 7,
        PendingFinalVetting = 8,
        ReadyForRelease = 9,
        ToBeReplacedWithExternalOntologyTerm = 10,
        OrganizationalTerm = 11,
        ExampleToBeEventuallyRemoved = 12
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ThesaurusMergeState
    {
        Pending,
        Completed,
        NotCompleted
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
        Professor = 1,
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
        Archived,
        Resolved,
        Rejected
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
    public enum QuestionOccurenceType
    {
        Any,
        Same,
        Different
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IterationState
    {
        NotStarted,
        Design,
        InProgress,
        Finished,
        Terminated
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
        OrganizationIdentifierType,
        CodeSystem,
        Country,
        AddressType,
        ReligiousAffiliationType,
        Citizenship
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GlobalUserSource
    {
        Microsoft,
        Google,
        Internal
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GlobalUserStatus
    {
        NotVerified,
        Active,
        Blocked
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredifinedGlobalUserRole
    {
        SuperAdministrator = 1,
        Viewer,
        Editor,
        Curator
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PredifinedRole
    {
        SuperAdministrator = 1,
        Administrator,
        Doctor
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NodeState
    {
        NotStarted,
        Active,
        Completed,
        Disabled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormInstanceViewMode
    {
        RegularView,
        SynopticView
    }

    public enum FieldSpecialValues
    {
        NE = -2147483646 // Not Done
    }
}
