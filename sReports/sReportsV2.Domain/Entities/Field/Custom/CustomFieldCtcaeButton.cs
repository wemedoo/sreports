using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FieldEntity.Custom.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity.Custom
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.CustomButton)]
    public class CustomFieldButton : FieldEntity.Field
    {
        public override string Type { get; set; } = FieldTypes.CustomButton;
        public CustomAction CustomAction { get; set; }
    }
}
