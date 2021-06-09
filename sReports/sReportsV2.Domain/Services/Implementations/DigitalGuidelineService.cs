using MongoDB.Driver;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class DigitalGuidelineService : IDigitalGuidelineService
    {
        private readonly IMongoCollection<Guideline> Collection;

        public DigitalGuidelineService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Guideline>("digitalguidelines");
        }
        public async Task<Guideline> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x.Id.Equals(id)).SingleAsync().ConfigureAwait(false);
        }

        public async Task InsertOrUpdateAsync(Guideline guideline)
        {
            if (string.IsNullOrEmpty(guideline.Id))
            {
                guideline.EntryDatetime = DateTime.Now;
                guideline.LastUpdate = DateTime.Now;
                guideline.Version = new Entities.Form.Version()
                {
                    Major = 1,
                    Minor = 1,
                    Id = Guid.NewGuid().ToString()
                };

                await Collection.InsertOneAsync(guideline).ConfigureAwait(false);
            }
            else
            {
                Guideline guidelineForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(guideline.Id));
                guideline.EntryDatetime = guidelineForUpdate.EntryDatetime;
                guidelineForUpdate.DoConcurrencyCheck(guideline.LastUpdate.Value);

                guideline.LastUpdate = DateTime.Now;
                var filter = Builders<Guideline>.Filter.Eq(s => s.Id, guideline.Id);
                var result = await Collection.ReplaceOneAsync(filter, guideline).ConfigureAwait(false);
            }
        }

        public List<Guideline> GetAll(GuidelineFilter filter)
        {
            return this.GetGuidelineFiltered(filter)
               .OrderByDescending(x => x.EntryDatetime)
               .Skip((filter.Page - 1) * filter.PageSize)
               .Take(filter.PageSize)
               .Select(x => new Guideline()
               {
                   Id = x.Id,
                   Version = x.Version,
                   LastUpdate = x.LastUpdate,
                   EntryDatetime = x.EntryDatetime,
                   Title = x.Title,
                   
               })
               .ToList();
        }

        public int GetAllCount(GuidelineFilter filter)
        {
            return this.GetGuidelineFiltered(filter).Count();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            Guideline forDelete = this.GetById(id);
            Entity.DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<Guideline>.Filter.Eq(x => x.Id, id);
            var update = Builders<Guideline>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        private Guideline GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id));
        }

        private IQueryable<Guideline> GetGuidelineFiltered(GuidelineFilter filter)
        {
            var result = Collection.AsQueryable()
                .Where(x => !x.IsDeleted);
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Title))
                    result = result.Where(x => x.Title.ToUpper().Contains(filter.Title));
                if (filter.Major != 0)
                    result = result.Where(x => x.Version.Major == filter.Major);
                if (filter.Minor != 0)
                    result = result.Where(x => x.Version.Minor == filter.Minor);
                if (filter.DateTimeTo != null)
                    result = result.Where(x => x.EntryDatetime < filter.DateTimeTo);
                if (filter.DateTimeFrom != null)
                    result = result.Where(x => x.EntryDatetime > filter.DateTimeFrom);
            }

            return result;
        }
    }
}
