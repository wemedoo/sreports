using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Regex)]
    public class FieldRegex : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Regex;
        public string Regex { get; set; }
        public string RegexDescription { get; set; }

    }
}
