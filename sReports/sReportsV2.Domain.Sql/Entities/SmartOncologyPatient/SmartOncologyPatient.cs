using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.Domain.Sql.Entities.Patient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.SmartOncologyPatient
{
    public class SmartOncologyPatient : PatientBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("SmartOncologyPatientId")]
        public int SmartOncologyPatientId { get; set; }
        public string IdentificationNumber { get; set; }
        // 0 .. * ?
        public string Allergies { get; set; }
        public string PatientInformedFor  { get; set; }
        public string PatientInformedBy { get; set; }
        public DateTime? PatientInfoSignedOn { get; set; }
        public DateTime? CopyDeliveredOn { get; set; }
        public int CapabilityToWork { get; set; }
        public bool DesireToHaveChildren { get; set; }
        public bool FertilityConservation { get; set; }
        public bool SemenCryopreservation { get; set; }
        public bool EggCellCryopreservation { get; set; }
        public bool SexualHealthAddressed { get; set; }
        public Contraception Contraception { get; set; }
        // 0 .. * ?
        public string ClinicalTrials { get; set; }
        public bool PreviousTreatment  { get; set; }
        public bool TreatmentInCantonalHospitalGraubunden  { get; set; }
        // 0 .. * ?
        public string HistoryOfOncologicalDisease  { get; set; }
        // 0 .. * ?
        public string HospitalOrPraxisOfPreviousTreatments { get; set; }
        public DiseaseContext DiseaseContextAtInitialPresentation { get; set; }
        public string StageAtInitialPresentation  { get; set; }
        public DiseaseContext DiseaseContextAtCurrentPresentation { get; set; }
        public string StageAtCurrentPresentation { get; set; }
        public string Anatomy { get; set; }
        public string Morphology { get; set; }
        public string TherapeuticContext { get; set; }
        public string ChemotherapyType { get; set; }
        public int ChemotherapyCourse  { get; set; }
        public int ChemotherapyCycle  { get; set; }
        public DateTime? FirstDayOfChemotherapy  { get; set; }
        // 0 .. * ?
        public int? ConsecutiveChemotherapyDays { get; set; }

        [Column("MultipleBirthId")]
        public int? MultipleBirthId { get; set; }
        [Column("ContactId")]
        public int? ContactId { get; set; }

        public void Copy(SmartOncologyPatient patient)
        {
            base.Copy(patient);

            this.IdentificationNumber = patient.IdentificationNumber;
            this.StageAtInitialPresentation = patient.StageAtInitialPresentation;
            this.StageAtCurrentPresentation = patient.StageAtCurrentPresentation;
            this.Anatomy = patient.Anatomy;
            this.Morphology = patient.Morphology;
            this.TherapeuticContext = patient.TherapeuticContext;
            this.ChemotherapyType = patient.ChemotherapyType;

            this.CapabilityToWork = patient.CapabilityToWork;
            this.ChemotherapyCourse = patient.ChemotherapyCourse;
            this.ChemotherapyCycle = patient.ChemotherapyCycle;

            this.PatientInfoSignedOn = patient.PatientInfoSignedOn;
            this.CopyDeliveredOn = patient.CopyDeliveredOn;
            this.FirstDayOfChemotherapy = patient.FirstDayOfChemotherapy;

            this.DesireToHaveChildren = patient.DesireToHaveChildren;
            this.FertilityConservation = patient.FertilityConservation;
            this.SemenCryopreservation = patient.SemenCryopreservation;
            this.EggCellCryopreservation = patient.EggCellCryopreservation;
            this.SexualHealthAddressed = patient.SexualHealthAddressed;
            this.PreviousTreatment = patient.PreviousTreatment;
            this.TreatmentInCantonalHospitalGraubunden = patient.TreatmentInCantonalHospitalGraubunden;

            this.Contraception = patient.Contraception;
            this.DiseaseContextAtInitialPresentation = patient.DiseaseContextAtInitialPresentation;
            this.DiseaseContextAtCurrentPresentation = patient.DiseaseContextAtCurrentPresentation;

            this.Allergies = patient.Allergies;
            this.HospitalOrPraxisOfPreviousTreatments = patient.HospitalOrPraxisOfPreviousTreatments;
            this.ConsecutiveChemotherapyDays = patient.ConsecutiveChemotherapyDays;

            this.PatientInformedBy = patient.PatientInformedBy;
            this.PatientInformedFor = patient.PatientInformedFor;
            this.HistoryOfOncologicalDisease = patient.HistoryOfOncologicalDisease;
            this.ClinicalTrials = patient.ClinicalTrials;
        }

        public List<int> GetClinicalTrialIds()
        {
            return GetRepetitiveValues(ClinicalTrials).Select(strValue => {
                bool success = int.TryParse(strValue, out int value);
                return value;
            }).ToList();
        }

        private List<string> GetRepetitiveValues(string values)
        {
            if (string.IsNullOrWhiteSpace(values))
                return new List<string>();
            else
                return values.Split(';').ToList();
        }
    }
}
