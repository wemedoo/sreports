using sReportsV2.Models.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter.DataOut
{
    public class EnumDataOut
    {
        public ThesaurusEntryViewModel Thesaurus { get; set; }
        public string Label { get; set; }
    }
}