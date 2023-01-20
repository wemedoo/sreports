using sReportsV2.Common.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class MedicationReplacementDataOut
    {
        public int Id { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        public MedicationInstancePreviewDataOut ReplaceMedication { get; set; }
        public int ReplaceMedicationId { get; set; }
        public MedicationInstancePreviewDataOut ReplaceWithMedication { get; set; }
        public int ReplaceWithMedicationId { get; set; }
        public DateTime EntryDatetime { get; set; }
        public UserData Creator { get; set; }
        public bool IsStartReplacement { get; set; }
    }
}
