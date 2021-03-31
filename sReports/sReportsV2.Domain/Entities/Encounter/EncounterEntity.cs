using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Encounter
{
    public class EncounterEntity: Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string EpisodeOfCareId { get; set; }
        public string Status { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string ServiceType { get; set; }
        public Period Period { get; set; }
        [BsonIgnore]
        public List<FormInstance.FormInstance> FormInstances { get; set; }

    }
}
