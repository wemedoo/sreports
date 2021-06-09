using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Initializer.PredefinedTypes
{
    public class EnumEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsDeleted { get; set; }
        public string Type { get; set; }
    }
}
