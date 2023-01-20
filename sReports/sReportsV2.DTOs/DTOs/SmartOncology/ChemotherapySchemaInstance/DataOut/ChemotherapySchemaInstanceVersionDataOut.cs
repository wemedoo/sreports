using sReportsV2.Common.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class ChemotherapySchemaInstanceVersionDataOut
    {
        public int Id { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        public UserData Creator { get; set; }
        public int CreatorId { get; set; }
        public int FirstDelayDay { get; set; }
        public int DelayFor { get; set; }
        public string ReasonForDelay { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime DelayedDate { get; set; }
    }
}
