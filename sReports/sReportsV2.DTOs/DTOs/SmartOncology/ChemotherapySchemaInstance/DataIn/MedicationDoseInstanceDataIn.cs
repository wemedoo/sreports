using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class MedicationDoseInstanceDataIn
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public DateTime? Date { get; set; }
        public List<MedicationDoseTimeInstanceDataIn> MedicationDoseTimes { get; set; } = new List<MedicationDoseTimeInstanceDataIn>();
        public int MedicationInstanceId { get; set; }
        public int? UnitId { get; set; }
        public int? IntervalId { get; set; }
        public string RowVersion { get; set; }
        public string MedicationName { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }

    }
}
