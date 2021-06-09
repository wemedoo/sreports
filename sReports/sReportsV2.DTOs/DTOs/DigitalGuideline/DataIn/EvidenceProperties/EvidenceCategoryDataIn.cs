using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DigitalGuideline.DataIn.EvidenceProperties
{
    public class EvidenceCategoryDataIn
    {
        public ThesaurusEntryDataOut StrengthOfRecommendation { get; set; }
        public ThesaurusEntryDataOut OxfordLevelOfEvidenceSystem { get; set; }
        public NCCNEvidenceCategoryDataIn NCCNEvidenceCategory { get; set; }
    }
}