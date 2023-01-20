using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.CustomFieldFilters
{
    [BsonIgnoreExtraElements]
    public class CustomFieldFilterGroup : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<CustomFieldFilterData> CustomFieldFiltersData { get; set; }
        public string FormDefinitonId { get; set; }
        public string OverallOperator { get; set; }
    }
}
