using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry.DTO
{
    public class ThesaurusEntryTranslationDTO
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Definition { get; set; }
        public string PreferredTerm { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> Abbreviations { get; set; }
    }
}