using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
    typeof(GuidelineStatementElementData),
    typeof(GuidelineDecisionElementData),
    typeof(GuidelineEdgeElementData),
    typeof(GuidelineEventElementData))]
    public class GuidelineElementData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        [BsonIgnore]
        public virtual string Type { get; set; }
        public EvidenceProperties.EvidenceProperties EvidenceProperties { get; set; }
    }
}
