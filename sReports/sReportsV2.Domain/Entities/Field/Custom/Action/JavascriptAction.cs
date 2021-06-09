using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity.Custom.Action
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(CustomActionTypes.JavascriptAction)]
    public class JavascriptAction : CustomAction
    {
        public override string ActionType { get; set; } = CustomActionTypes.JavascriptAction;

        public string MethodName { get; set; }
    }
}
