using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry
{
    public class ThesaurusEntryFilterDataIn : sReportsV2.DTOs.Common.DataIn
    { 
        public int Id { get; set; }
        public string PreferredTerm { get; set; }
        public string Synonym { get; set; }
        public string SimilarTerm { get; set; }
        public string Abbreviation { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public ThesaurusState? State { get; set; }
        public int ActiveThesaurus { get; set; }
        public string ActiveLanguage { get; set; }
        public List<string> ThesaurusStates { get; set; } = new List<string>();
    }
}