using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class EncounterService : IEncounterService
    {
        private readonly IMongoCollection<EncounterEntity> Encounter;
        private readonly IMongoCollection<ServiceType> ServiceType;

        public EncounterService()
        {
            IMongoDatabase MongoDatabase = Mongo.MongoDBInstance.Instance.GetDatabase();
            Encounter = MongoDatabase.GetCollection<EncounterEntity>("encounterentity") as IMongoCollection<EncounterEntity>;
            ServiceType = MongoDatabase.GetCollection<ServiceType>("servicetype") as IMongoCollection<ServiceType>;
        }

        public long GetAllEntriesCount()
        {
            return Encounter.Find(x => !x.IsDeleted).CountDocuments();
        }

        public EncounterEntity GetById(string id)
        {
            return Encounter.Find(x => !x.IsDeleted && x.Id.Equals(id)).FirstOrDefault();
        }

        public List<EncounterEntity> GetByIds(List<string> ids)
        {
            return Encounter.Find(x => !x.IsDeleted && ids.Contains(x.Id)).ToList();
        }

        public long GetAllEntriesCountByEocId(string id)
        {
            return Encounter.Find(x => x.IsDeleted.Equals(false) && x.EpisodeOfCareId.Equals(id)).CountDocuments();
        }

        public List<EncounterEntity> GetAll(int pageSize, int page)
        {
            return Encounter
                .Find(x => x.IsDeleted.Equals(false))
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
        }

        public List<EncounterEntity> GetAllByEocId(int pageSize, int page, string eocId)
        {
            return Encounter
                .Find(x => x.IsDeleted.Equals(false) && x.EpisodeOfCareId.Equals(eocId))
                .SortByDescending(x => x.EntryDatetime)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
        }
       
        public string Insert(EncounterEntity encounter)
        {
            encounter = Ensure.IsNotNull(encounter, nameof(encounter));

            if (encounter.Id == null)
            {
                encounter.EntryDatetime = DateTime.Now;
                encounter.LastUpdate = DateTime.Now;
                this.Encounter.InsertOne(encounter);
            }
            else
            {
                EncounterEntity encounterForUpdate = Encounter.AsQueryable().FirstOrDefault(x => x.Id.Equals(encounter.Id));
                encounter.EntryDatetime = encounterForUpdate.EntryDatetime;
                encounterForUpdate.DoConcurrencyCheck(encounter.LastUpdate.Value);

                encounter.LastUpdate = DateTime.Now;
                var filter = Builders<EncounterEntity>.Filter.Eq(s => s.Id, encounter.Id);
                var result = this.Encounter.ReplaceOne(filter, encounter).ModifiedCount;
            }

            return encounter.Id;
        }

        public List<ServiceType> GetAllServiceTypes()
        {
            return this.ServiceType.AsQueryable().Where(x => !x.IsDeleted)
                                                 .OrderBy(x => x.Display).ToList();
        }

        public List<EncounterEntity> GetByObjectId(string id)
        {
            return Encounter.Find(x => x.Id.Equals(id) && !x.IsDeleted).ToList();
        }

        public List<EncounterEntity> GetByParameters(EncounterFhirFilter encounterFilter)
        {
            return Encounter.Find(x => (encounterFilter.Status == null || x.Status.Equals(encounterFilter.Status))
                                                    && (encounterFilter == null || x.Type.Equals(encounterFilter.Type))
                                                    && (encounterFilter.Class == null || x.Class.Equals(encounterFilter.Class))
                                                    && !x.IsDeleted).ToList();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            EncounterEntity encounterForDelete = GetById(id);
            DoConcurrencyCheckForDelete(encounterForDelete);
            encounterForDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<EncounterEntity>.Filter.Eq(x => x.Id, id);
            var update = Builders<EncounterEntity>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Encounter.UpdateOne(filter, update).IsAcknowledged;
        }

        public bool ExistEncounter(string encounterId)
        {
            return Encounter.Find(x => !x.IsDeleted && x.Id.Equals(encounterId)).CountDocuments() > 0;
        }
        private void DoConcurrencyCheckForDelete(EncounterEntity forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }

        public EncounterEntity GetLatestByEpisodeOFCare(string episodeOfCareId)
        {
            return Encounter.AsQueryable().Where(e => e.EpisodeOfCareId.Equals(episodeOfCareId)).OrderByDescending(x => x.EntryDatetime).FirstOrDefault();
        }
    }
}
