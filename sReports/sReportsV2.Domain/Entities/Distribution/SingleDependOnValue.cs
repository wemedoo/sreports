using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class SingleDependOnValue
    {
        public string Id { get; set; }
        public string FieldLabel { get; set; }
        public string Value { get; set; } // single option
        public string ValueLabel { get; set; }
        public string Type { get; set; }
    }
}
