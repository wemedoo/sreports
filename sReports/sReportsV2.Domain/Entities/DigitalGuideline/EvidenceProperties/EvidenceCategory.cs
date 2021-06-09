using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DigitalGuideline.EvidenceProperties
{
    public class EvidenceCategory
    {
        public string StrengthOfRecommendation { get; set; }
        public string OxfordLevelOfEvidenceSystem { get; set; }
        public NCCNEvidenceCategory NCCNEvidenceCategory { get; set; }
    }
}
