using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn
{
    public class GuidelineEdgeElementDataDataIn : GuidelineElementDataDataIn
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Condition { get; set; }
    }
}