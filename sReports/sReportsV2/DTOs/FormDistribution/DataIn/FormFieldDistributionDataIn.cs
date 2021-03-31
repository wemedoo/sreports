using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class FormFieldDistributionDataIn
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string ThesaurusId { get; set; }
        public List<RelatedVariable> RelatedVariables { get; set; }
        public List<FormFieldDistributionSingleParameterDataIn> Values { get; set; }

    }
}