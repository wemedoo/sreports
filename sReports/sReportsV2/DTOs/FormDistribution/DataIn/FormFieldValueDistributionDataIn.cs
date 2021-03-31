using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class FormFieldValueDistributionDataIn
    {
        public string Label { get; set; }
        public string ThesaurusId { get; set; }
        public string Value { get; set; }
        public double SuccessProbability { get; set; }
    }
}