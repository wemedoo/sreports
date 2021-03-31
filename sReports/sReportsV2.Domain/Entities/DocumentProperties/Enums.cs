﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DocumentProperties
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
        AccidentAndEmergencyMedicine,
        Alleorgology,
        Anaestetics,
        Cardiology,
        ChildPsyhiatry,
        ClinicalBilology,
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
        VeterinaryMedicine
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
