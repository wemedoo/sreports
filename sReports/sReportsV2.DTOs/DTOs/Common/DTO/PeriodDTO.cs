using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class PeriodDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override string ToString()
        {
            return $"{StartDate.ToString("dd/MM/yyyy")} - " + (EndDate != null ? EndDate.Value.ToString("dd/MM/yyyy") : string.Empty);
        }


        public string StartToTimeZonedString(string offset)
        {
          return  StartDate.ToTimeZoned(offset).ToString("s");
        }

        public string EndToTimeZonedString(string offset)
        {
            return EndDate != null ? EndDate.Value.ToTimeZoned(offset).ToString("s") : string.Empty;
        }
    }
}