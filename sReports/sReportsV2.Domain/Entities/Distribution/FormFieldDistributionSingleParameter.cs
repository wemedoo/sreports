using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class FormFieldDistributionSingleParameter
    {
        public List<SingleDependOnValue> DependOn { get; set; }
        public FormFieldNormalDistributionParameters NormalDistributionParameters { get; set; }
        public List<FormFieldValueDistribution> Values { get; set; }
    }
}
