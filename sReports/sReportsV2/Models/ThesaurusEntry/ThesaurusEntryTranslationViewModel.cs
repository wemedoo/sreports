using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.ThesaurusEntry
{
    public class ThesaurusEntryTranslationViewModel
    {
        public string Language { get; set; }
        public string Definition { get; set; }
        public string PreferredTerm { get; set; }
        public List<string> SimilarTerms { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Abbreviations { get; set; }
    }
}