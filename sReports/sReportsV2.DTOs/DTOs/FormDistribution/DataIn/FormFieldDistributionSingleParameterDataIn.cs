using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class FormFieldDistributionSingleParameterDataIn
    {        
        public List<SingleDependOnValueDataIn> DependOn { get; set; }
        public FormFieldNormalDistributionParametersDataIn NormalDistributionParameters { get; set; }
        public List<FormFieldValueDistributionDataIn> Values { get; set; }

    }
}