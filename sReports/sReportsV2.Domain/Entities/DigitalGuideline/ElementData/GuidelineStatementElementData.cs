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
    [BsonDiscriminator(GuidelineElementDataTypes.Statement)]
    public class GuidelineStatementElementData : GuidelineElementData
    {
        public override string Type { get; set; } = GuidelineElementDataTypes.Statement;
    }
}
