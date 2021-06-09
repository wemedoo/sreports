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
    [BsonDiscriminator(CustomActionTypes.ControllerAction)]
    public class ControllerAction : CustomAction
    {
        public override string ActionType { get; set; } = CustomActionTypes.ControllerAction;
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
}
