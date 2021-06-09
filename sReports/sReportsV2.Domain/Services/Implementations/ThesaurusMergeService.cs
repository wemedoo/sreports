using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class ThesaurusMergeService : IThesaurusMergeService
    {
        private readonly IMongoCollection<ThesaurusMerge> Collection;
        private readonly IMongoDatabase MongoDatabase;
        public ThesaurusMergeService()
        {
            MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<ThesaurusMerge>("thesaurusmerge") as IMongoCollection<ThesaurusMerge>;
        }

        public List<ThesaurusMerge> GetAllByState(ThesaurusMergeState state)
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted && x.State == state).ToList();
        }

        public void InsertOrUpdate(ThesaurusMerge thesaurus)
        {
            thesaurus = Ensure.IsNotNull(thesaurus, nameof(thesaurus));
            
            if (thesaurus.Id == null)
            {
                thesaurus.EntryDatetime = DateTime.Now;
                thesaurus.LastUpdate = DateTime.Now;
                Collection.InsertOne(thesaurus);
            }
            else
            {
                thesaurus.LastUpdate = DateTime.Now;
                var filter = Builders<ThesaurusMerge>.Filter.Eq(s => s.Id, thesaurus.Id);
                var result = Collection.ReplaceOne(filter, thesaurus).ModifiedCount;
            }
        }
    }
}
