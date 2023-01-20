using MongoDB.Driver;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class CustomFieldFilterDAL : ICustomFieldFilterDAL
    {
        private readonly IMongoCollection<CustomFieldFilterGroup> Collection;

        public CustomFieldFilterDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<CustomFieldFilterGroup>("customfieldfilter");
        }

        public string InsertOrUpdateCustomFieldFilter(CustomFieldFilterGroup customFieldFilterGroup)
        {
            customFieldFilterGroup = Ensure.IsNotNull(customFieldFilterGroup, nameof(customFieldFilterGroup));

            if (customFieldFilterGroup.Id == null)
            {
                customFieldFilterGroup.EntryDatetime = DateTime.Now;
                customFieldFilterGroup.LastUpdate = DateTime.Now;
                Collection.InsertOne(customFieldFilterGroup);
            }
            else
            {
                CustomFieldFilterGroup customFieldFilterGroupDb = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(customFieldFilterGroup.Id));
                customFieldFilterGroup.DoConcurrencyCheck(customFieldFilterGroup.LastUpdate.Value);
                customFieldFilterGroup.EntryDatetime = customFieldFilterGroup.EntryDatetime;
                customFieldFilterGroup.LastUpdate = DateTime.Now;
                var filter = Builders<CustomFieldFilterGroup>.Filter.Eq(s => s.Id, customFieldFilterGroup.Id);
                var result = Collection.ReplaceOne(filter, customFieldFilterGroup).ModifiedCount;
            }
            return customFieldFilterGroup.Id;
        }

        public List<CustomFieldFilterGroup> GetCustomFieldFiltersByFormId(string formDefinitionId)
        {
            IQueryable<CustomFieldFilterGroup> result =  Collection.AsQueryable(new AggregateOptions() { AllowDiskUse = true });
            return result.Where(x => x.FormDefinitonId == formDefinitionId).ToList();
        }

    }
}
