using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn
{
    public class GuidelineFilterDataIn: Common.DataIn
    {
        public string Title { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public DateTime? DateTimeFrom { get; set; }
    }
}