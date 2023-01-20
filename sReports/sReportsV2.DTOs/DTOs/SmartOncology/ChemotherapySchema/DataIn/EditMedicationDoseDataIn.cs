using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class EditMedicationDoseDataIn
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public string RowVersion { get; set; }
        public string MedicationName { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        public int DayNumber { get; set; }
    }
}
