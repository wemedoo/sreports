using sReportsV2.DTOs.ThesaurusEntry.DataOut;

namespace sReportsV2.DTOs.DigitalGuideline.DataOut.EvidenceProperties
{
    public class EvidenceCategoryDataOut
    {
        public ThesaurusEntryDataOut StrengthOfRecommendation { get; set; }
        public ThesaurusEntryDataOut OxfordLevelOfEvidenceSystem { get; set; }
        public NCCNEvidenceCategoryDataOut NCCNEvidenceCategory { get; set; }
    }
}