using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class EditMedicationDoseInBatchDataOut
    {
        public Dictionary<string, int> IdsPerDays { get; set; }
        public string RowVersion { get; set; }
    }
}
