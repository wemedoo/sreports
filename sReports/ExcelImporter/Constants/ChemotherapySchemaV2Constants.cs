using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImporter.Constants
{
    public static class ChemotherapySchemaV2Constants
    {
        public const string SchemaName = "Chemotherapy schema name";
        public const string IndicationAnatomicalTumorName = "Indication - Anatomical Tumor Name";
        public const string IndicationMorphologicalTumorType = "Indication - Morphologycal Tumor Type";
        public const string IndicationTumorStage = "Indication - Tumor Stage";
        public const string IndicationTreatmentIntent = "Indication - Treatment Intent";
        public const string MedicationName = "Medication Name";
        public const string IsSupportiveMedication = "Is supportive medication?";
        public const string Amount = "Amount";
        public const string Unit = "Unit";
        public const string RouteOfAdministration = "Route of administration";
        public const string PreparationInstruction = "Preparation Instruction";
        public const string ApplicationInstruction = "Application Instruction";
        public const string PremedicationDays = "Premedication days";
        public const string NumberOfDays = "Number of days";
        public const string IsDoseSame = "Is dose the same for every application of medication?";
        public const string HasMaximalCumulative = "Has maximal cumulative dose:";
        public const string MaximalCumulativeDose = "Maximal cumulative dose:";
        public const string LiteratureReference = "Literature reference";
    }
}
