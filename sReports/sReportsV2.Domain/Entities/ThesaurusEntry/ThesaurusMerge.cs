using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    public class ThesaurusMerge : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int NewThesaurus { get; set; }
        public int OldThesaurus { get; set; }
        public ThesaurusMergeState State { get; set; }
        public List<string> CompletedCollections { get; set; }
        public List<string> FailedCollections { get; set; }
    }
}
