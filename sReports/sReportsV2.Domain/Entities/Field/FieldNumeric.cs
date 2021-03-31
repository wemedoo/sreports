using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Number)]
    public class FieldNumeric : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Number;
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }
    }
}
