using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class ThesaurusEntryFilterData
    {
        public string O40MtId { get; set; }
        public string PreferredTerm { get; set; }
        public string Synonym { get; set; }
        public string SimilarTerm { get; set; }
        public string Abbreviation { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public ThesaurusState? State { get; set; }
    }
}
