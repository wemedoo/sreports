using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class MedicationDoseDataIn
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public List<MedicationDoseTimeDataIn> MedicationDoseTimes { get; set; } = new List<MedicationDoseTimeDataIn>();
        public int MedicationId { get; set; }
        public int? IntervalId { get; set; }
        public int? UnitId { get; set; }
        public string RowVersion { get; set; }
    }
}
