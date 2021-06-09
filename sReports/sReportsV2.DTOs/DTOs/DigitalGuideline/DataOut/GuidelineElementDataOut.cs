using sReportsV2.DTOs.DigitalGuideline.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataOut
{
    public class GuidelineElementDataOut
    {
        public string Group { get; set; }
        public bool Removed { get; set; }
        public bool Selected { get; set; }
        public bool Selectable { get; set; }
        public bool Locked { get; set; }
        public bool Grabbable { get; set; }
        public bool Pannable { get; set; }
        public string Classes { get; set; }
        public PositionDTO Position { get; set; }
        public GuidelineElementDataDataOut Data { get; set; }
    }
}