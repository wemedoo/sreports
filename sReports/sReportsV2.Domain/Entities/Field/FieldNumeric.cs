using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Number)]
    public class FieldNumeric : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Number;
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Step { get; set; }
    }
}
