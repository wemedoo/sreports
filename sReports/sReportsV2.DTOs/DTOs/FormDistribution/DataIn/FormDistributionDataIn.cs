using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataIn
{
    public class FormDistributionDataIn
    {
        public string Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<FormFieldDistributionDataIn> Fields { get; set; }
        public string FormDistributionId { get; set; }
        public int ThesaurusId { get; set; }
        public string VersionId { get; set; }

    }
}