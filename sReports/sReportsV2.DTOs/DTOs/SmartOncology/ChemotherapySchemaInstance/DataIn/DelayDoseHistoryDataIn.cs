using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class DelayDoseHistoryDataIn
    {
        public int DayNumber { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
