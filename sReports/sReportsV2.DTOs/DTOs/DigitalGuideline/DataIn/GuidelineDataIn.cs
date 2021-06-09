using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn
{
    public class GuidelineDataIn
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ThesaurusEntryDataOut Thesaurus { get; set; }
        public DateTime? LastUpdate { get; set; }
        public GuidelineElementsDataIn GuidelineElements { get; set; }
        public VersionDTO Version { get; set; }

    }

    public class GuidelineElementsDataIn
    {
        public List<GuidelineElementDataIn> Nodes { get; set; }
        public List<GuidelineElementDataIn> Edges { get; set; }
    }
}