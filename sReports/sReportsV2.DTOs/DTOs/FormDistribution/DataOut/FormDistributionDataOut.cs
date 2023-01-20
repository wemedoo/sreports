using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormDistribution.DataOut
{
    public class FormDistributionDataOut
    {
        public string Id { get; set; }
        public string ThesaurusId { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<FormFieldDistributionDataOut> Fields { get; set; }
        public string VersionId { get; set; }

        public FormFieldDistributionDataOut GetFieldById(string id)
        {
            return Fields.FirstOrDefault(f => f.Id == id);
        }

    }
}