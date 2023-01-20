using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class ChemotherapySchemaInstanceDataIn
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public List<MedicationInstanceDataIn> Medications { get; set; } = new List<MedicationInstanceDataIn>();
        public int PatientId { get; set; }
        public int ChemotherapySchemaId { get; set; }
        public string RowVersion { get; set; }
    }
}
