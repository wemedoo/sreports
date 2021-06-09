using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormFieldValueDistributionDataOut
    {
        public string Label { get; set; }
        public int ThesaurusId { get; set; }
        public string Value { get; set; }
        public float? SuccessProbability { get; set; }
    }
}