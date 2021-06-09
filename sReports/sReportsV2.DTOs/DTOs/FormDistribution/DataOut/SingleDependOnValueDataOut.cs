using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class SingleDependOnValueDataOut
    {
        public string Id { get; set; }
        public string FieldLabel { get; set; }
        public string Value { get; set; } // single option
        public string ValueLabel { get; set; }
        public string Type { get; set; }
    }
}