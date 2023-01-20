using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class ChemotherapySchemaInstanceDataOut
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public List<MedicationInstanceDataOut> Medications { get; set; } = new List<MedicationInstanceDataOut>();
        public ChemotherapySchemaPreviewDataOut ChemotherapySchema { get; set; }
        public SmartOncologyPatientDataOut Patient { get; set; }
        public int PatientId { get; set; }
        public int CreatorId { get; set; }
        public int ChemotherapySchemaId { get; set; }
        public SchemaTableDataOut SchemaTableData { get; set; }
        public string RowVersion { get; set; }
        public List<ChemotherapySchemaInstanceHistoryDataOut> History { get; set; } = new List<ChemotherapySchemaInstanceHistoryDataOut>();
        public bool IsNotPreview(){
            return Id == 0;
        }
    }
}
