using MongoDB.Driver;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormDistributionService : IFormDistributionService
    {
        private readonly IMongoCollection<FormDistribution> Collection;
        public FormDistributionService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FormDistribution>("formdistribution") as IMongoCollection<FormDistribution>;
        }

        public IQueryable<FormDistribution> GetAll(int page, int pageSize)
        {
            return Collection.AsQueryable().Skip((page - 1) * pageSize).Take(pageSize);
        }


        public List<FormDistribution> GetAll()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).ToList();
        }

        public List<FormDistribution> GetAllVersionAndThesaurus()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).Select(x=> new FormDistribution() { VersionId = x.VersionId, ThesaurusId = x.ThesaurusId }).ToList();
        }

        public int GetAllCount()
        {
            return Collection.AsQueryable().Count();
        }

        public FormDistribution GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id));
        }

        public FormDistribution GetByThesaurusId(int id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(id));
        }

        public FormDistribution GetByThesaurusIdAndVersion(int id, string versionId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(id) && x.VersionId == versionId);
        }

        public FormDistribution InsertOrUpdate(FormDistribution formDistribution)
        {
            formDistribution = Ensure.IsNotNull(formDistribution, nameof(formDistribution));

            if (string.IsNullOrEmpty(formDistribution.Id))
            {
                formDistribution.EntryDatetime = DateTime.Now;
                formDistribution.LastUpdate = DateTime.Now;
                Collection.InsertOne(formDistribution);
            }
            else
            {
                Update(formDistribution);
            }

            return formDistribution;
        }

        public bool ExistThesaurus(int thesaurusId)
        {
            return Collection.AsQueryable()
                                .Where(form => !form.IsDeleted && 
                                        form.Fields.Any(
                                            field => field.Values.Any(
                                                value => value.ThesaurusId == thesaurusId) 
                                            || field.ThesaurusId == thesaurusId
                                            || field.ValuesAll.Any(val => val.Values.Any(v => v.ThesaurusId == thesaurusId)))
                                        || form.ThesaurusId == thesaurusId
                                ).Count() > 1;
        }

        private FormDistribution Update(FormDistribution formDistribution)
        {
            FormDistribution forUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formDistribution.Id));
            formDistribution.EntryDatetime = forUpdate.EntryDatetime;
            formDistribution.LastUpdate = DateTime.Now;
            FilterDefinition<FormDistribution> filter = Builders<FormDistribution>.Filter.Eq(s => s.Id, formDistribution.Id);
            var result = Collection.ReplaceOne(filter, formDistribution).ModifiedCount;
            return formDistribution;
        }
    }
}
