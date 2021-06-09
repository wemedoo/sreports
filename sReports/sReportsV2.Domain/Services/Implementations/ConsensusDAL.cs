using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Insert(Consensus consensus)
        {
            consensus = Ensure.IsNotNull(consensus, nameof(consensus));

            if (consensus.Id == null)
            {
                consensus.EntryDatetime = DateTime.Now;
                consensus.LastUpdate = DateTime.Now;
                //consensus.UserRefs = new List<string>();
                //consensus.OutsideUserRefs = new List<string>();
                consensus.State = ConsensusFindingState.OnGoing;
                consensus.Iterations = new List<Entities.Consensus.ConsensusIteration>()
                {
                    new Entities.Consensus.ConsensusIteration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserIds = new List<int>(),
                        OutsideUserIds = new List<int>(),
                        Questions = new List<ConsensusQuestion>(),
                        State = IterationState.NotStarted
                    }
                };
                Collection.InsertOne(consensus);
            }
            else
            {
                Consensus consensusForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(consensus.Id));
                consensus.EntryDatetime = consensusForUpdate.EntryDatetime;
                consensus.LastUpdate = DateTime.Now;
                var filter = Builders<Consensus>.Filter.Eq(s => s.Id, consensus.Id);
                var result = Collection.ReplaceOne(filter, consensus).ModifiedCount;
            }
        }

        public void InsertQuestion(ConsensusQuestion question, string formId, string iterationId)
        {
            Consensus consensus = this.GetByFormId(formId);
            consensus.Iterations.Last().Questions.Add(question);
            this.Insert(consensus);
        }

        public bool IsLastIterationFinished(string consensusId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id == consensusId).Iterations.Last().State == IterationState.Finished;
        }

        private bool ConsensusExistByFormId(string formId) 
        {
            return Collection.AsQueryable().Any(x => x.FormRef == formId);
        }
    }
}
