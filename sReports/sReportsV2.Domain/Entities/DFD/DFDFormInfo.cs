using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DFD
{
    public class DFDFormInfo : Entity
    { 
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormThesaurusId { get; set; }
        public string FormTitle { get; set; }
        public string Language { get; set; }

    }
}
