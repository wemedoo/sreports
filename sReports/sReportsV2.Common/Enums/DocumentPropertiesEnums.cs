using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Enums.DocumentPropertiesEnums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentGeneralPurposeEnum
    {
        InformationCollection,
        InformationPresentation,
        MixedInformationPresentationAndCollection,
        ContextDependent
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContextDependent
    {
        None,
        UserAccessRight,
        DocumentInformationCollectionOrPresentationState
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentExplicitPurpose
    {
        ReferralToProcedure,
        ReportingOfProcedure,
        ReportingOfFindings,
        ReportingOfTherapoDiagnosticProcedures,
        CombinedPurposeForReferralAndReporting
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentScopeOfValidityEnum
    {
        International,
        AdministrativeUnit,
        InterInstitutional,
        IntraInstitutional
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentClinicalDomain
    {
        Allergy = 1,
        Acupuncture,
        AddictionMedicine,
        AddictionPsychiatry,
        AdolescentMedicine,
        AerodigestiveMedicine,
        AerospaceMedicine,
        Anesthesiology,
        Audiology,
        BariatricSurgery,
        BirthDefects,
        BloodBankingAndTransfusionMedicine,
        BoneMarrowTransplant,
        BrainInjury,
        BurnManagement,
        CardiacSurgery,
        CardiovascularDisease,
        ChemicalPathology,
        ChildAndAdolescentPsychiatry,
        ChildAndAdolescentPsychology,
        ChiropracticMedicine,
        CleftAndCraniofacial,
        ClinicalBiochemicalGenetics,
        ClinicalCardiacElectrophysiology,
        ClinicalGenetics,
        ClinicalPathology,
        ClinicalPharmacology,
        ColonAndRectalSurgery,
        CommunityHealthCare,
        CriticalCareMedicine,
        Dentistry,
        DevelopmentalBehavioralPediatrics,
        Diabetology,
        Dialysis,
        EatingDisorders,
        EmergencyMedicine,
        EnvironmentalHealth,
        Epilepsy,
        Ethics,
        FamilyMedicine,
        ForensicMedicine,
        GeneralMedicine,
        GeriatricMedicine,
        GynecologicOncology,
        Gynecology,
        HeartFailure,
        Hepatology,
        HIV,
        InfectiousDisease,
        IntegrativeMedicine,
        InterventionalCardiology,
        InterventionalRadiology,
        Kinesiotherapy,
        MaternalAndFetalMedicine,
        MedicalAidInDying,
        MedicalGenetics,
        MedicalMicrobiologyPathology,
        MedicalOncology,
        MedicalToxicology,
        MentalHealth,
        MultiSpecialtyProgram,
        NeonatalPerinatalMedicine,
        NeurologicalSurgery,
        NeurologyWSpecialQualificationsInChildNeuro,
        Neuropsychology,
        NutritionAndDietetics,
        ObesityMedicine,
        Obstetrics,
        ObstetricsAndGynecology,
        OccupationalTherapy,
        Optometry,
        OrthopaedicSurgery,
        OrthoticsProsthetics,
        Otolaryngology,
        PainMedicine,
        PalliativeCare,
        PastoralCare,
        PediatricCardiology,
        PediatricCriticalCareMedicine,
        PediatricDermatology,
        PediatricEndocrinology,
        PediatricGastroenterology,
        PediatricHematologyOncology,
        PediatricInfectiousDiseases,
        PediatricNephrology,
        PediatricOtolaryngology,
        PediatricPulmonology,
        PediatricRehabilitationMedicine,
        PediatricRheumatology,
        Pediatrics,
        PediatricSurgery,
        PediatricTransplantHepatology,
        PediatricUrology,
        Pharmacogenomics,
        PhysicalMedicineAndRehab,
        PhysicalTherapy,
        Podiatry,
        Polytrauma,
        PrimaryCare,
        Psychology,
        PulmonaryDisease,
        RecreationalTherapy,
        ReproductiveEndocrinologyAndInfertility,
        Research,
        RespiratoryTherapy,
        SleepMedicine,
        SolidOrganTransplant,
        SpeechLanguagePathology,
        SpinalCordInjuryMedicine,
        SpinalSurgery,
        SportsMedicine,
        Surgery,
        SurgeryOfTheHand,
        SurgicalCriticalCare,
        SurgicalOncology,
        TherapeuticApheresis,
        ThoracicAndCardiacSurgery,
        Thromboembolism,
        TransplantCardiology,
        TransplantSurgery,
        Trauma,
        TumorBoard,
        UnderseaAndHyperbaricMedicine,
        VascularNeurology,
        VocationalRehabilitation,
        WomensHealth,
        WoundCareManagement,
        WoundOstomyAndContinenceCare,

        AccidentAndEmergencyMedicine,
        Allergology,
        Anaestetics,
        Cardiology,
        ChildPsyhiatry,
        ClinicalBiology,
        ClinicalChemistry,
        ClinicalNeurophysiology,
        CraniofacialSurgery,
        Dermatology,
        Endocrinology,
        FamilyAndGeneralMedicine,
        GastroenterologicSurgery,
        Gastroenterology,
        GeneralPractice,
        GeneralSurgery,
        Geriatrics,
        Hematology,
        Immunology,
        InfectiousDiseases,
        InternalMedicine,
        LaboratoryMedicine,
        Microbiology,
        Nephrology,
        Neuropsychiatry,
        Neurology,
        Neurosurgery,
        NuclearMedicine,
        ObstetricsAndGynaecology,
        OccupationalMedicine,
        Oncology,
        Ophthalmology,
        OralAndMaxillofacialSurgery,
        Orthopaedics,
        Otorhinolaryngology,
        PaediatricSurgery,
        Paediatrics,
        Pathology,
        Pharmacology,
        PhysicalMedicineAndRehabilitation,
        PlasticSurgery,
        PodiatricSurgery,
        PreventiveMedicine,
        Psychiatry,
        PublicHealth,
        RadiationOncology,
        Radiology,
        RespiratoryMedicine,
        Rheumatology,
        Stomatology,
        ThoracicSurgery,
        TropicalMedicine,
        Urology,
        VascularSurgery,
        Venereology,
        PreClinicalResearch,
        VeterinaryMedicine,
        ClinicalMicrobiology
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentClinicalContextEnum
    {
        Preventive,
        Screening,
        Diagnostic,
        Theragnostic,
        Therapy,
        Rehabilitation,
        FollowUp
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FollowUp
    {
        None,
        EarlyDetectionOfDiseaseRelapse,
        EvaluationOfTherapeuticEffect,
        DetectionAndEvalutationOfTherapyAssociatedAdverseEvents,
        TreatmentOfTherapyaAssociatedAdverseEvents
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AdministrativeContext
    {
        InsuranceRelatedDocumentation,
        Billing,
        RegistryEntry
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentClassEnum
    {
        Clinical,
        Research,
        AdministrativeMedical,
        Other
    }
}
