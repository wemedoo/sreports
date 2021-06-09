using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class FormFieldValueDistribution
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public int ThesaurusId { get; set; }
        public float? SuccessProbability { get; set; }
    }
}
