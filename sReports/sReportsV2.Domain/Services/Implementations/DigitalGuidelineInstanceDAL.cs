using MongoDB.Driver;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.DigitalGuidelineInstance;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sReportsV2.Domain.Services.Implementations
{
    public class DigitalGuidelineInstanceDAL : IDigitalGuidelineInstanceDAL
    {
        private readonly IMongoCollection<GuidelineInstance> Collection;

        public DigitalGuidelineInstanceDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<GuidelineInstance>("digitalguidelineinstance") as IMongoCollection<GuidelineInstance>;
        }

        public GuidelineInstance GetById(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).FirstOrDefault();
        }

        public void Insert(GuidelineInstance guidelineInstance)
        {
            guidelineInstance = Ensure.IsNotNull(guidelineInstance, nameof(guidelineInstance));

            if (guidelineInstance.Id == null)
            {
                guidelineInstance.Copy(null);
                guidelineInstance.Period = new Entities.Common.Period()
                {
                    Start = DateTime.Now.Date
                };
                Collection.InsertOne(guidelineInstance);
            }
            else
            {
                GuidelineInstance forUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(guidelineInstance.Id));
                forUpdate.DoConcurrencyCheck(guidelineInstance.LastUpdate.Value);
                guidelineInstance.Copy(forUpdate);
                FilterDefinition<GuidelineInstance> filter = Builders<GuidelineInstance>.Filter.Eq(s => s.Id, guidelineInstance.Id);
                var result = Collection.ReplaceOne(filter, guidelineInstance).ModifiedCount;
            }
        }

        public  List<GuidelineInstance> GetGuidelineInstancesByEOC(int episodeOfCareId)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.EpisodeOfCareId == episodeOfCareId)
                .ToList();
        }

        public async Task<List<GuidelineInstance>> GetGuidelineInstancesByEOCAsync(int episodeOfCareId) 
        {
            return await Collection
                .Find(x => !x.IsDeleted && x.EpisodeOfCareId == episodeOfCareId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public bool Delete(string guidelineInstanceId)
        {
            GuidelineInstance forDelete = GetById(guidelineInstanceId);
            DoConcurrencyCheckForDelete(forDelete);

            var filter = Builders<GuidelineInstance>.Filter.Eq(x => x.Id, guidelineInstanceId);
            var update = Builders<GuidelineInstance>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public string GetNodeValue(string nodeId, string guidelineInstanceId)
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted && x.Id.Equals(guidelineInstanceId))
                                           .Select(x => x.NodeValues.Where(o => o.Id.Equals(nodeId)))
                                           .FirstOrDefault().Select(o => o.Value).FirstOrDefault();
        }

        public NodeState GetNodeState(string nodeId, string guidelineInstanceId)
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted && x.Id.Equals(guidelineInstanceId))
                                           .Select(x => x.NodeValues.Where(o => o.Id.Equals(nodeId)))
                                           .FirstOrDefault().Select(o => o.State).FirstOrDefault();
        }

        private void DoConcurrencyCheckForDelete(GuidelineInstance forDelete)
        {
            if (forDelete == null)
            {
                throw new ConcurrencyDeleteException();
            }
        }
    }
}
