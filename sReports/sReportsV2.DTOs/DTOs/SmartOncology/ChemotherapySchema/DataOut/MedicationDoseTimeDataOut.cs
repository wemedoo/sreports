using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut
{
    public class MedicationDoseTimeDataOut
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Dose { get; set; }
        public int MedicationDoseId { get; set; }
    }
}
