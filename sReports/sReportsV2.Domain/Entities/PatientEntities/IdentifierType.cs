using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{

    [BsonIgnoreExtraElements]
    public class IdentifierType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string O4MtId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
