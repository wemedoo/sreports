using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization.DataIn;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn
{
    public class SmartOncologyPatientDataIn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool Activity { get; set; }

        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public ContactDTO ContactPerson { get; set; }
        public List<IdentifierDataIn> Identifiers { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public List<CommunicationDTO> Communications { get; set; }

        // smartOncologyPatient
        public string IdentificationNumber { get; set; }
        public string Allergies { get; set; }
        public string PatientInformedFor { get; set; }
        public string PatientInformedBy { get; set; }
        public DateTime? PatientInfoSignedOn { get; set; }
        public DateTime? CopyDeliveredOn { get; set; }
        public int CapabilityToWork { get; set; }
        public bool DesireToHaveChildren { get; set; }
        public bool FertilityConservation { get; set; }
        public bool SemenCryopreservation { get; set; }
        public bool EggCellCryopreservation { get; set; }
        public bool SexualHealthAddressed { get; set; }
        public string Contraception { get; set; }
        public string ClinicalTrials { get; set; }
        public bool PreviousTreatment { get; set; }
        public bool TreatmentInCantonalHospitalGraubunden { get; set; }
        public string HistoryOfOncologicalDisease { get; set; }
        public string HospitalOrPraxisOfPreviousTreatments { get; set; }
        public string DiseaseContextAtInitialPresentation { get; set; }
        public string StageAtInitialPresentation { get; set; }
        public string DiseaseContextAtCurrentPresentation { get; set; }
        public string StageAtCurrentPresentation { get; set; }
        public string Anatomy { get; set; }
        public string Morphology { get; set; }
        public string TherapeuticContext { get; set; }
        public string ChemotherapyType { get; set; }
        public int ChemotherapyCourse { get; set; }
        public int ChemotherapyCycle { get; set; }
        public DateTime? FirstDayOfChemotherapy { get; set; }
        public int? ConsecutiveChemotherapyDays { get; set; }
    }
}
