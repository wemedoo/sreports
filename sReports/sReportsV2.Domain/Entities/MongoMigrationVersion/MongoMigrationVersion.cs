using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.MongoMigrationVersion
{
    [BsonIgnoreExtraElements]
    public class MongoMigrationVersion : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }    
    }
}
