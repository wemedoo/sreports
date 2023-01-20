using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class DependentVariableRelatedVariables
    {
        public int ThesaurusId { get; set; }
        public string TargetVariable { get; set; }
        public string VersionId { get; set; }
        public List<RelatedVariable> RelatedVariables { get; set; }
        public string FormDistributionId { get; set; }

    }
}