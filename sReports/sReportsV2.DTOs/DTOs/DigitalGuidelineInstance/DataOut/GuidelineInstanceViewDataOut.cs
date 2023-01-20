using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuidelineInstance.DataOut
{
    public class GuidelineInstanceViewDataOut
    {
        public GuidelineInstanceDataOut GuidelineInstance { get; set; }
        public List<GuidelineDataOut> Guidelines { get; set; }
        public List<FormInstanceDataOut> FormInstances { get; set; }
    }
}