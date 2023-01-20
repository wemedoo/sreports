using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class MedicationInstanceDataOut
    {
        public int Id { get; set; }
        public MedicationPreviewDataOut Medication { get; set; }
        public int MedicationId { get; set; }
        public List<MedicationDoseInstanceDataOut> MedicationDoses { get; set; } = new List<MedicationDoseInstanceDataOut>();
    }
}
