using sReportsV2.Domain.Entities.Constants;
using sReportsV2.DTOs.FormDistribution.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormFieldDistributionDataOut
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string ThesaurusId { get; set; }
        public List<FormFieldDistributionSingleParameterDataOut> ValuesCombination { get; set; }
        public List<RelatedVariable> RelatedVariables { get; set; }

        public bool IsSelectableField()
        {
            return Type == FieldTypes.Select || Type == FieldTypes.Checkbox || Type == FieldTypes.Radio;
        }
    }
}