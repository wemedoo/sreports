using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Common.DTO
{
    public class PeriodDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [DateRange]
        public DateTime? EndDate { get; set; }

        public string EndToTimeZonedString(string offset, string dateFormat)
        {
            return EndDate != null ? EndDate.Value.ToTimeZoned(offset).ToString(dateFormat) : string.Empty;
        }

        public string EndToTimeZonedTimeString(string offset)
        {
            return EndDate != null ? EndDate.Value.ToTimeZoned(offset).GetTimePart() : string.Empty;
        }
    }
}