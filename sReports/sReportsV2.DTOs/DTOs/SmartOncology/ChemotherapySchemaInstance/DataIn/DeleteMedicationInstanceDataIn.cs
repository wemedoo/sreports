using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class DeleteMedicationInstanceDataIn
    {
        public int Id { get; set; }
        public string RowVersionMedication { get; set; }
        public string RowVersion { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
    }
}
