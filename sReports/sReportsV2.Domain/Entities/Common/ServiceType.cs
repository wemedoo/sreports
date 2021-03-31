using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sReportsV2.Domain.Entities.Common
{
    public class ServiceType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Display { get; set; }
        public string Definition { get; set; }
        public bool IsDeleted { get; set; }
        public string ThesaurusId { get; set; }
        public ServiceType() { }
    }
}
