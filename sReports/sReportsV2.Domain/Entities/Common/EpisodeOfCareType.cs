using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class EpisodeOfCareType : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    
        public string Display { get; set; }

        public string Code { get; set; }

        public string CodeSystem { get; set; }

        public string Definition { get; set; }

        public int FHIRResource { get; set; }

        public EpisodeOfCareType() { }
    }
}
