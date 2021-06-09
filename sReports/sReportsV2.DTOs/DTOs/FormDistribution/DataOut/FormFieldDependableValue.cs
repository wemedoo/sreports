using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormFieldDependableValue
    {
        public float SuccessProbability { get; set; }
        public List<int> DependOn { get; set; }
    }
}