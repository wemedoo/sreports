using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    [BsonIgnoreExtraElements]
    public class Guideline : Entity
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public GuidelineElements GuidelineElements { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }

    }

    public class GuidelineElements
    {
        public List<GuidelineElement> Nodes { get; set; }
        public List<GuidelineElement> Edges { get; set; }
    }
}
