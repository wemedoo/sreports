using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class Comment : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public CommentState CommentState { get; set; }
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public string CommentRef { get; set; }
        public string FormRef { get; set; }
        public int UserId { get; set; }

    }
}
