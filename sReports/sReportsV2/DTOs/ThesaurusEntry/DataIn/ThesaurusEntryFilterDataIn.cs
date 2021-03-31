using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    public class ThesaurusEntryFilterDataIn : DataIn
    {
        public string O40MtId { get; set; }
        public string PreferredTerm { get; set; }
        public string Synonym { get; set; }
        public string SimilarTerm { get; set; }
        public string Abbreviation { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
    }
}