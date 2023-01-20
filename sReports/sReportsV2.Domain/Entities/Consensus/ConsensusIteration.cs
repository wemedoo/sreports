using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace sReportsV2.Domain.Entities.Consensus
{
    public class ConsensusIteration
    {
        public string Id { get; set; }
        public List<ConsensusQuestion> Questions { get; set; }
        public List<int> UserIds { get; set; }
        public List<int> OutsideUserIds { get; set; }
        public IterationState? State { get; set; }
        public List<QuestionOccurenceConfig> QuestionOccurences { get; set; }
        [BsonDateTimeOptions(Representation = BsonType.Document)]
        public DateTime EntryDatetime { get; set; }
    }
}
