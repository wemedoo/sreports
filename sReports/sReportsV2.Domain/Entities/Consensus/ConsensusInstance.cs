using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Consensus
{
    public class ConsensusInstance : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ConsensusRef { get; set; }
        public int UserId { get; set; }
        public bool IsOutsideUser { get; set; }
        public List<ConsensusQuestion> Questions { get; set; }
        public string IterationId { get; set; }

        public double GetPercentDone() 
        {
            return Questions.Count > 0 ?
                Math.Round(this.Questions.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Count() / (double)Questions.Count * 100, 2)
                : 0;
        }

        public bool IsCompleted()
        {
            return GetPercentDone() == 100;
        }
    }
}
