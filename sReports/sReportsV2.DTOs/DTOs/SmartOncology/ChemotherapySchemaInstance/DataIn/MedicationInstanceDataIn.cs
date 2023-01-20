using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class MedicationInstanceDataIn
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public MedicationDataIn Medication { get; set; }
        public List<MedicationDoseInstanceDataIn> MedicationDoses { get; set; } = new List<MedicationDoseInstanceDataIn>();
        public int ChemotherapySchemaInstanceId { get; set; }
        public List<int> MedicationIdsToReplace { get; set; } = new List<int>();
    }
}
