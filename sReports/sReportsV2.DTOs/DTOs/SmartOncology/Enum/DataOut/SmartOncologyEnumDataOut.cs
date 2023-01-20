using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut
{
    public class SmartOncologyEnumDataOut
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public static class SmartOncologyEnumNames {
        public const string Anatomy = "Diagnosis - Anatomy";
        public const string Morphology = "Diagnosis - Morphology";
        public const string TherapeuticContext = "Type of therapy";
        public const string PresentationStage = "Diagnosis - Stage Grouping";
        public const string ChemotherapyType = "Chemotherapy Schema Name";
        public const string DayNumber = "Day number";
        public const string MedicationName = "Medication Name";
        public const string Dose = "Dose";
        public const string PreparationInstruction = "Preparation Instruction";
        public const string ApplicationInstruction= "Application Instruction";
        public const string Notes = "Notes";
        public const string TreatmentType = "Treatment Type";
        public const string CancerDirectedTreatment = "Cancer Directed Treatment";
        public const string SupportiveTherapy = "Supportive Therapy";


        public const string DiagnosesSheet = "Diagnoses";
        public const string TherapyCategorizationSheet = "Therapy categorisation";
        public const string ChemotherapySchemasSheet = "Chemotherapy Schemas";

        public const string sReportsVocabularyFileName = "sReports Vocabulary and FHIR Resources";
        public const string ChemotherapyCompendiumFileName = "Chemotherapy_Compendium";
    }
}
