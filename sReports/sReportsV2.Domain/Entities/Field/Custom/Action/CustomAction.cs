using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity.Custom.Action
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
        typeof(ControllerAction),
        typeof(CustomAction))]
    public class CustomAction
    {
        [BsonIgnore]
        public virtual string ActionType { get; set; }
    }
}
