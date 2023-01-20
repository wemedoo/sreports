using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class ConsensusDAL : IConsensusDAL
    {
        private readonly IMongoCollection<Consensus> Collection;

        public ConsensusDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Consensus>("consensus");
        }

        public bool CanStartConsensusFindingProcess(string consensusId)
        {
            return GetById(consensusId).Iterations.Last().State == IterationState.Design;
        }

        public Consensus GetByFormId(string formId)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.FormRef.Equals(formId))
                .FirstOrDefault();
        }

        public Consensus GetById(string id)
        {
            return Collection
                            .Find(x => !x.IsDeleted && x.Id.Equals(id))
                            .FirstOrDefault();
        }

        public ConsensusIteration GetLastIteration(string consensusId)
        {
            return Collection.AsQueryable().Where(x => x.Id == consensusId).Select(x => x.Iterations.Last()).SingleOrDefault();
        }

        public void Insert(Consensus consensus)
        {
            consensus = Ensure.IsNotNull(consensus, nameof(consensus));

            if (consensus.Id == null)
            {
                consensus.Copy(null);
                Collection.InsertOne(consensus);
            }
            else
            {
                Consensus consensusForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(consensus.Id));
                consensus.Copy(consensusForUpdate);
                var filter = Builders<Consensus>.Filter.Eq(s => s.Id, consensus.Id);
                var result = Collection.ReplaceOne(filter, consensus).ModifiedCount;
            }
        }

        public bool IsLastIterationFinished(string consensusId)
        {
            IterationState? currentIterationState = GetLastIterationState(consensusId);
            return currentIterationState == IterationState.Finished || currentIterationState == IterationState.Terminated;
        }

        public IterationState GetLastIterationState(string consensusId)
        {
            IterationState? currentIterationState = Collection.AsQueryable().Where(x => x.Id == consensusId).Select(x => x.Iterations.Last().State).SingleOrDefault();
            return currentIterationState ?? IterationState.NotStarted;
        }
    }
}
