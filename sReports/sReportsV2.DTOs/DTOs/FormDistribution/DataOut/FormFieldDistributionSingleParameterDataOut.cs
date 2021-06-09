using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormFieldDistributionSingleParameterDataOut
    {
        public List<SingleDependOnValueDataOut> DependOn { get; set; }
        public FormFieldNormalDistributionParametersDataOut NormalDistributionParameters { get; set; }
        public List<FormFieldValueDistributionDataOut> Values { get; set; }
    }
}