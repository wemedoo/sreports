using sReportsV2.Common.Entities.User;
using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class ChemotherapySchemaInstancePreviewDataOut
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public UserData Creator { get; set; }
        public DateTime EntryDatetime { get; set; }
        public ChemotherapySchemaPreviewDataOut ChemotherapySchema {get; set; }
        public InstanceState State { get; set; }
    }
}
