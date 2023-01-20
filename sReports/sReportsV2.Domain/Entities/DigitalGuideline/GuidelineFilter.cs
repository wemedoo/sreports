using System;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    public class GuidelineFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Title { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public string ColumnName { get; set; }
        public bool IsAscending { get; set; }

    }
}
