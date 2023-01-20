using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaTableMedicationInstanceDataOut
    {
        public int Id { get; set; }
        public SchemaTableMedicationDataOut Medication { get; set; }
        public int MedicationId { get; set; }
        public List<MedicationDoseInstanceDataOut> MedicationDoses { get; set; } = new List<MedicationDoseInstanceDataOut>();
        public int ChemotherapySchemaInstanceId { get; set; }
        public string RowVersion { get; set; }
    }
}
