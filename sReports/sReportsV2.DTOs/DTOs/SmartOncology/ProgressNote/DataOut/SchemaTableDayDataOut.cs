using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut
{
    public class SchemaTableDayDataOut
    {
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsDelayed { get; set; }
    }
}
