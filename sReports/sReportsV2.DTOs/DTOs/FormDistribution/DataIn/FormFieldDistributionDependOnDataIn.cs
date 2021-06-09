using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class FormFieldDistributionDependOnDataIn
    {
        public string Id { get; set; }
        public float? UpperBoundary { get; set; }
        public float? LowerBoundary { get; set; }
    }
}