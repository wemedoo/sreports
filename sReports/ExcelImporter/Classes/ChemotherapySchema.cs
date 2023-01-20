using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImporter.Classes
{
    public class ChemotherapySchema
    {
        public string SchemaName { get; set; }
        public string IndicationAnatomicalTumorName { get; set; }
        public string IndicationMorphologicalTumorType { get; set; }
        public string IndicationTumorStage { get; set; }
        public string IndicationTreatmentIntent { get; set; }
        public string MedicationName { get; set; }
        public string IsSupportiveMedication { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
        public string RouteOfAdministration { get; set; }
        public string PreparationInstruction { get; set; }
        public string ApplicationInstruction { get; set; }
        public string PremedicationDays { get; set; }
        public string NumberOfDays { get; set; }
        public string IsDoseSame { get; set; }
        public string HasMaximalCumulative { get; set; }
        public string MaximalCumulativeDose { get; set; }
        public string LiteratureReference { get; set; }
    }
}
