using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.ThesaurusEntry;
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
    public class ThesaurusEntryService : IThesaurusEntryService
    {
        private readonly IMongoCollection<ThesaurusEntry> Collection;
        private readonly IMongoDatabase MongoDatabase;
        public ThesaurusEntryService()
        {
            MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<ThesaurusEntry>("thesaurusentry") as IMongoCollection<ThesaurusEntry>;
        }

        public ThesaurusEntry GetById(string id)
        {
            return Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).FirstOrDefault();
        }

        public string InsertOrUpdate(ThesaurusEntry thesaurusEntry, UserData user)
        {
            thesaurusEntry = Ensure.IsNotNull(thesaurusEntry, nameof(thesaurusEntry));

            if (thesaurusEntry.Id != null)
            {
                UpdateThesaurusEntry(thesaurusEntry, user);
            }
            else
            {
                InsertThesaurusEntry(thesaurusEntry, user);
            }
            return thesaurusEntry.O40MTId;
        }

        public bool ExistsThesaurusEntry(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                result = Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).CountDocuments() > 0;
            }

            return result;
        }

        public List<ThesaurusEntry> GetAll(ThesaurusEntryFilterData filterData)
        {
            filterData = Ensure.IsNotNull(filterData, nameof(filterData));

            return GetThesaurusEntriesFiltered(filterData)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filterData.Page - 1) * filterData.PageSize)
                .Take(filterData.PageSize)
                .ToList();
        }

        public long GetAllEntriesCount(ThesaurusEntryFilterData filterData)
        {
            return GetThesaurusEntriesFiltered(filterData).Count();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            ThesaurusEntry thesaurusForDelete = GetById(id);
            DoConcurrencyCheckForDelete(thesaurusForDelete);
            thesaurusForDelete.DoConcurrencyCheck(lastUpdate);
            
            var filter = Builders<ThesaurusEntry>.Filter.Eq(x => x.Id, id);
            var update = Builders<ThesaurusEntry>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public long GetNextSequenceValue()
        {

            var collection = MongoDatabase.GetCollection<Sequence>("sequence");
            var filter = Builders<Sequence>.Filter.Eq(a => a.Id, "autoIncrement");
            var update = Builders<Sequence>.Update.Inc(a => a.Value, 1);
            var sequence = collection.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence> { ReturnDocument = ReturnDocument.After, IsUpsert = true });

            return sequence.Value;
        }

        private void UpdateThesaurusEntry(ThesaurusEntry thesaurusEntry, UserData user)
        {
            ThesaurusEntry entry = Collection.AsQueryable().FirstOrDefault(x => !x.IsDeleted && x.Id.Equals(thesaurusEntry.Id));
            entry.DoConcurrencyCheck(thesaurusEntry.LastUpdate.Value);

            thesaurusEntry.EntryDatetime = entry.EntryDatetime;
            thesaurusEntry.LastUpdate = DateTime.Now;
            thesaurusEntry.AdministrativeData = entry.AdministrativeData ?? new AdministrativeData();
            thesaurusEntry.AdministrativeData.UpdateVersionHistory(user);
            var filter = Builders<ThesaurusEntry>.Filter.Where(s => s.Id.Equals(thesaurusEntry.Id));
            Collection.ReplaceOne(filter, thesaurusEntry);
        }

        private void InsertThesaurusEntry(ThesaurusEntry thesaurusEntry, UserData user)
        {
            thesaurusEntry.EntryDatetime = DateTime.Now;
            thesaurusEntry.AdministrativeData = new AdministrativeData(user);
            thesaurusEntry.O40MTId = GetNextSequenceValue().ToString();
            thesaurusEntry.LastUpdate = DateTime.Now;

            Collection.InsertOne(thesaurusEntry);
        }

        public void InsertThesaurusEntryWithId(ThesaurusEntry thesaurusEntry, UserData user)
        {
            thesaurusEntry = Ensure.IsNotNull(thesaurusEntry, nameof(thesaurusEntry));

            if(Collection.AsQueryable().Any(x => x.O40MTId.Equals(thesaurusEntry.O40MTId)))
            {
                return;
            }

            thesaurusEntry.EntryDatetime = DateTime.Now;
            thesaurusEntry.AdministrativeData = new AdministrativeData(user);
            thesaurusEntry.LastUpdate = DateTime.Now;

            if (string.IsNullOrWhiteSpace(thesaurusEntry.O40MTId))
            {
                thesaurusEntry.O40MTId = GetNextSequenceValue().ToString();
            }
            else
            {
                GetNextSequenceValue();
            }

            Collection.InsertOne(thesaurusEntry);
        }

        private IQueryable<ThesaurusEntry> GetThesaurusEntriesFiltered(ThesaurusEntryFilterData filterData)
        {
            IQueryable<ThesaurusEntry> result = Collection.AsQueryable().Where(x => !x.IsDeleted);
            if(filterData != null)
            {
                result = result.Where(x => (filterData.O40MtId == null || x.O40MTId.Equals(filterData.O40MtId))
                        && (filterData.UmlsCode == null || x.UmlsCode.Equals(filterData.UmlsCode))
                    );
                if (!string.IsNullOrEmpty(filterData.PreferredTerm))
                {
                    result = result.Where(x => x.Translations.Any(y => y.PreferredTerm.ToUpper(CultureInfo.InvariantCulture).Contains(filterData.PreferredTerm.ToUpper(CultureInfo.InvariantCulture))));
                }

                if (!string.IsNullOrEmpty(filterData.UmlsName))
                {
                    result = result.Where(x => x.UmlsName.ToUpper(CultureInfo.InvariantCulture).Contains(filterData.UmlsName.ToUpper(CultureInfo.InvariantCulture)));
                }

                if (!string.IsNullOrEmpty(filterData.Abbreviation))
                {
                    result = result.Where(x => x.Translations.Any(y => y.Abbreviations.Any(a => a.ToUpper(CultureInfo.InvariantCulture).Contains(filterData.Abbreviation.ToUpper(CultureInfo.InvariantCulture)))));
                }

                if (!string.IsNullOrEmpty(filterData.SimilarTerm))
                {
                    result = result.Where(x => x.Translations.Any(y => y.SimilarTerms.Any(a => a.ToUpper(CultureInfo.InvariantCulture).Contains(filterData.SimilarTerm.ToUpper(CultureInfo.InvariantCulture)))));
                }

                if (!string.IsNullOrEmpty(filterData.Synonym))
                {
                    result = result.Where(x => x.Translations.Any(y => y.Synonyms.Any(a => a.ToUpper(CultureInfo.InvariantCulture).Contains(filterData.Synonym.ToUpper(CultureInfo.InvariantCulture)))));
                }
            }

            return result;
        }

        public bool ExistsThesaurusEntryByO4MtId(string o4MtId)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(o4MtId))
            {
                result = Collection.Find(x => !x.IsDeleted && x.O40MTId.Equals(o4MtId)).CountDocuments() > 0;
            }

            return result;
        }

        public ThesaurusEntry GetByO4MtIdId(string o4MtId)
        {
            return Collection.Find(x => !x.IsDeleted && x.O40MTId.Equals(o4MtId)).FirstOrDefault();

        }
        public List<ThesaurusEntry> GetByIdsList(List<string> thesaurusList)
        {
            return Collection.AsQueryable().Where(x => thesaurusList.Contains(x.O40MTId) && !x.IsDeleted).ToList();
        }
        public long GetUmlsEntriesCount()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted && !string.IsNullOrEmpty(x.UmlsCode)).Count();
        }
        private void DoConcurrencyCheckForDelete(ThesaurusEntry forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }
    }
}
