using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry.DataIn
{
    public class ThesaurusMergeDataIn
    {
        public string CurrentId { get; set; }
        public string TargetId { get; set; }
        public List<string> ValuesForMerge { get; set; }
    }
}