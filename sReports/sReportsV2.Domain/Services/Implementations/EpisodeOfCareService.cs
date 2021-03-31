using MongoDB.Driver;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class EpisodeOfCareService : IEpisodeOfCareService
    {
        private readonly IMongoCollection<EpisodeOfCareEntity> Collection;
        private readonly IMongoCollection<EpisodeOfCareType> EpisodeOfCareType;
        private readonly IMongoCollection<EncounterEntity> EncounterCollection;

        public EpisodeOfCareService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<EpisodeOfCareEntity>("episodeofcareentity") as IMongoCollection<EpisodeOfCareEntity>;
            EpisodeOfCareType = MongoDatabase.GetCollection<EpisodeOfCareType>("episodeofcaretypes") as IMongoCollection<EpisodeOfCareType>;
            EncounterCollection = MongoDatabase.GetCollection<EncounterEntity>("encounterentity") as IMongoCollection<EncounterEntity>;
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            EpisodeOfCareEntity forDelete = GetEOCById(id);
            DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<EpisodeOfCareEntity>.Filter.Eq(x => x.Id, id);
            var update = Builders<EpisodeOfCareEntity>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public long GetAllEntriesCountByPatientId(string id, string organizationId)
        {
            return Collection.Find(x => x.PatientId.Equals(id) && !x.IsDeleted && x.OrganizationRef.Equals(organizationId)).CountDocuments();
        }

        public List<EpisodeOfCareEntity> GetAllByIds(List<string> ids)
        {
            return Collection.AsQueryable().Where(x => ids.Contains(x.Id)).ToList();
        }

        public List<EpisodeOfCareEntity> GetAll(EpisodeOfCareFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return GetFiltered(filter)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
        }

        public EpisodeOfCareEntity GetByEncounter(string encounterId)
        {
            string eocId = EncounterCollection.AsQueryable().FirstOrDefault(x => x.Id.Equals(encounterId))?.EpisodeOfCareId;
            return this.Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(eocId));
        }

        public List<EpisodeOfCareEntity> GetAllForPatientInPeriod(string patientId, DateTime dataTimeStart, DateTime dataTimeEnd)
        {
            return Collection.Find(x => x.PatientId.Equals(patientId) && !x.IsDeleted  && x.Period.Start > dataTimeStart && x.Period.Start < dataTimeEnd).ToList();
        }

        public List<EpisodeOfCareEntity> GetAllByPatientId(int pageSize, int page, string patientId, string organizationId)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.PatientId.Equals(patientId) && x.OrganizationRef.Equals(organizationId))
                .SortByDescending(x => x.EntryDatetime)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
        }

        public EpisodeOfCareEntity GetEOCById(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).FirstOrDefault();
        }

        public List<EpisodeOfCareEntity> GetByPatientId(string patientId)
        {
            return Collection.Find(x => x.PatientId.Equals(patientId) && !x.IsDeleted).ToList();
        }

        public List<EpisodeOfCareEntity> GetByParameters(EpisodeOfCareFhirFilter episodeOfCareFilter)
        {
            return Collection.Find(x => (episodeOfCareFilter.Status == null ||  x.Status.Equals(episodeOfCareFilter.Status))
                                                    && (episodeOfCareFilter.Type == null || x.Type.Equals(episodeOfCareFilter.Type)) 
                                                    && (episodeOfCareFilter.Condition == null || x.DiagnosisCondition.Equals(episodeOfCareFilter.Condition))
                                                    && !x.IsDeleted
                                                    ).ToList();
        }

        public List<EpisodeOfCareStatus> GetStatusHistory(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).FirstOrDefault().ListHistoryStatus;
        }

        public string InsertOrUpdate(EpisodeOfCareEntity episodeOfCare)
        {
            episodeOfCare = Ensure.IsNotNull(episodeOfCare, nameof(episodeOfCare));

            if (episodeOfCare.Id == null)
            {
                episodeOfCare.EntryDatetime = DateTime.Now;
                episodeOfCare.LastUpdate = episodeOfCare.EntryDatetime;
                Collection.InsertOne(episodeOfCare);
            }
            else
            {
                EpisodeOfCareEntity forUpdate = GetEOCById(episodeOfCare.Id.ToString());
                forUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(episodeOfCare.Id));
                episodeOfCare.EntryDatetime = forUpdate.EntryDatetime;
                forUpdate.DoConcurrencyCheck(episodeOfCare.LastUpdate.Value);

                episodeOfCare.LastUpdate = DateTime.Now;
                var filter = Builders<EpisodeOfCareEntity>.Filter.Eq(s => s.Id, episodeOfCare.Id);
                var result = Collection.ReplaceOne(filter, episodeOfCare).ModifiedCount;
            }

            return episodeOfCare.Id;
        }

        public long GetAllEntriesCount(EpisodeOfCareFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return GetFiltered(filter).Count();
        }

        public bool ExistById(string id)
        {
            return Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).CountDocuments() > 0;
        }

        private IQueryable<EpisodeOfCareEntity> GetFiltered(EpisodeOfCareFilter filter)
        {
            IQueryable<EpisodeOfCareEntity> filteredData = Collection.AsQueryable().Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(filter.Description))
            {
                filteredData = filteredData.Where(x => x.Description.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Description));
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                filteredData = filteredData.Where(x => x.Status.Equals(filter.Status));
            }
            if (filter.PeriodStartDate != null)
            {
                DateTime beginStartDate = filter.PeriodStartDate ?? DateTime.Now;
                DateTime endStartDate = beginStartDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.Start >= beginStartDate && x.Period.Start < endStartDate);
            }
            if (filter.PeriodEndDate != null)
            {
                DateTime beginEndDate = filter.PeriodEndDate ?? DateTime.Now;
                DateTime endEndDate = beginEndDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.End >= beginEndDate && x.Period.End < endEndDate);
            }
            if (!string.IsNullOrEmpty(filter.Type))
            {
                filteredData = filteredData.Where(x => x.Type.Equals(filter.Type));
            }
            if (filter.FilterByIdentifier)
            {
                filteredData = filteredData.Where(x => x.PatientId.Equals(filter.PatientId));
            }
            if (!string.IsNullOrWhiteSpace(filter.OrganizationId)) 
            {
                filteredData = filteredData.Where(x => x.OrganizationRef.Equals(filter.OrganizationId));
            }

            return filteredData;
        }

        private void DoConcurrencyCheckForDelete(EpisodeOfCareEntity forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }
    }
}
