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
    [BsonDiscriminator(FieldTypes.Datetime)]
    public class FieldDatetime : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Datetime;
    }
}
