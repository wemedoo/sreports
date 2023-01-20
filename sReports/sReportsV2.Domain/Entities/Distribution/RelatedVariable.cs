using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class RelatedVariable
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public float? UpperBoundary { get; set; }
        public float? LowerBoundary { get; set; }
    }
}
