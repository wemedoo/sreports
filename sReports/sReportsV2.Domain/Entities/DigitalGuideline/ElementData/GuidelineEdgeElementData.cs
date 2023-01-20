using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(GuidelineElementDataTypes.Edge)]
    public class GuidelineEdgeElementData : GuidelineElementData
    {
        public override string Type { get; set; } = GuidelineElementDataTypes.Edge;
        public string Source { get; set; }
        public string Target { get; set; }
        public string Condition { get; set; }

        public bool SatisfyCondition(string condition)
        {
            return Condition != null && Condition.Equals(condition);
        }
    }
}
