using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class EditMedicationDoseInBatchDataIn
    {
        public List<MedicationDoseDataIn> Doses { get; set; }
        public string RowVersion { get; set; }
        public int MedicationId { get; set; }
    }
}
