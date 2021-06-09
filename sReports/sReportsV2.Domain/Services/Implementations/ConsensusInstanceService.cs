using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class ConsensusInstanceService : IConsensusInstanceService
    {
        private readonly IMongoCollection<ConsensusInstance> Collection;

        public ConsensusInstanceService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<ConsensusInstance>("consensusInstance");
        }

        public List<ConsensusInstance> GetAllByConsensusId(string consensusId)
        {
            return Collection.AsQueryable().Where(x => x.ConsensusRef == consensusId).ToList();
        }

        public ConsensusInstance GetByConsensusAndUserAndIteration(string consensusId, int userId, string iterationId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ConsensusRef == consensusId && x.UserId == userId && x.IterationId == iterationId);
        }


        public ConsensusInstance GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id == id);
        }

        public void InsertOrUpdate(ConsensusInstance consensusInstance)
        {
            consensusInstance = Ensure.IsNotNull(consensusInstance, nameof(consensusInstance));

            if (consensusInstance.Id == null)
            {
                consensusInstance.EntryDatetime = DateTime.Now;
                consensusInstance.LastUpdate = DateTime.Now;
                Collection.InsertOne(consensusInstance);
            }
            else
            {
                ConsensusInstance consensusForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(consensusInstance.Id));
                consensusInstance.EntryDatetime = consensusForUpdate.EntryDatetime;
                consensusInstance.LastUpdate = DateTime.Now;
                var filter = Builders<ConsensusInstance>.Filter.Eq(s => s.Id, consensusInstance.Id);
                var result = Collection.ReplaceOne(filter, consensusInstance).ModifiedCount;
            }
        }
    }
}
