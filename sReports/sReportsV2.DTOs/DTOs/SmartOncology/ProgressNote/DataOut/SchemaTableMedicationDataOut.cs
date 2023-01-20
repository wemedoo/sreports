using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaTableMedicationDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Dose { get; set; }
        public UnitDTO Unit { get; set; }
        public string PreparationInstruction { get; set; }
        public string ApplicationInstruction { get; set; }
        public string AdditionalInstruction { get; set; }
        public bool IsSupportiveMedication { get; set; }
        public List<MedicationDoseDataOut> MedicationDoses { get; set; } = new List<MedicationDoseDataOut>();
        public int ChemotherapySchemaId { get; set; }
    }
}
