using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    public class ThesaurusReviewFilterDataIn :sReportsV2.DTOs.Common.DataIn
    {
        public int Id { get; set; }
        public string PreferredTerm { get; set; }
    }
}