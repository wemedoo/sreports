using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class DelayDoseDataIn
    {
        public int DayNumber { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        public int DelayFor { get; set; }
        public string ReasonForDelay { get; set; }
    }
}
