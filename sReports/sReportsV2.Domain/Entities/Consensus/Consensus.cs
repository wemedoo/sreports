using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class Consensus : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormRef { get; set; }
        public int CreatorId { get; set; }
        public ConsensusFindingState? State { get; set; }
        public List<ConsensusIteration> Iterations { get; set; }

        public List<int> GetAllUserIds(bool isOutsideUsers) 
        {
            List<int> result = new List<int>();
            foreach (ConsensusIteration iteration in this.Iterations) 
            {
                if (!isOutsideUsers)
                {
                    result.AddRange(iteration.UserIds);
                }
                else 
                {
                    result.AddRange(iteration.OutsideUserIds);
                }
            }

            return result.Distinct().ToList();
        }

        public void SetIterationsState(string newIterationId) 
        {
            foreach (ConsensusIteration iteration in this.Iterations.Where(x => !x.Id.Equals(newIterationId))) 
            {
                iteration.State = iteration.State != IterationState.Terminated ? IterationState.Finished : IterationState.Terminated;
            }
        }

        public ConsensusIteration GetIterationById(string id)
        {
            return Iterations.FirstOrDefault(x => x.Id.Equals(id));         
        }
    }
}
