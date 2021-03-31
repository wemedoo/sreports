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
    }
}