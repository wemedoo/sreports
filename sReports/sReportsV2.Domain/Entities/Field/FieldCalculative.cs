using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Calculative)]
    public class FieldCalculative : Field
    {
        public override string Type { get; set; } = FieldTypes.Calculative;
        public string Formula { get; set; }
        public Dictionary<string, string> IdentifiersAndVariables { get; set; }
    }
}
