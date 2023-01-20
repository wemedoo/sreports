using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut
{
    public class SmartOncologyPatientDataOut
    {
        // common props
        public int Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }

        // patient props
        public List<IdentifierDataOut> Identifiers { get; set; }
        public bool Activity { get; set; }
        public bool MultipleBirth { get; set; }
        public int MultipleBirthNumber { get; set; }
        public string ContactName { get; set; }
        public string ContactFamily { get; set; }
        public string ContactGender { get; set; }
        public string Relationship { get; set; }
        public string Language { get; set; }

        public List<TelecomDTO> ContactTelecoms { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public AddressDTO ContactAddress { get; set; }
        public List<CommunicationDTO> Communications { get; set; }
        public DateTime? LastUpdate { get; set; }

        public List<EpisodeOfCareDataOut> EpisodeOfCares { get; set; }


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
        public List<ClinicalTrialDTO> ClinicalTrials { get; set; } = new List<ClinicalTrialDTO>();
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

        public List<string> GetRepetitiveValues(string values)
        {
            if (string.IsNullOrWhiteSpace(values))
                return new List<string>();
            else
                return values.Split(';').ToList();
        }

        public string GetName()
        {
            return $"{Name} {FamilyName}";
        }
    }
}
