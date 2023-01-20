using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class ThesaurusEntryTranslation
    {
        public string Language { get; set; }
        public string Definition { get; set; }
        public string PreferredTerm { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Abbreviations { get; set; }
    }
}