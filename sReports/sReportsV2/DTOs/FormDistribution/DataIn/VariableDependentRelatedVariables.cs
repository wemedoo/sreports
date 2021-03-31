using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class DependentVariableRelatedVariables
    {
        public string ThesaurusId { get; set; }
        public string TargetVariable { get; set; }
        public List<RelatedVariable> RelatedVariables { get; set; }
    }
}