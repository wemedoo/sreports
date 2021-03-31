using MongoDB.Driver;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class EncounterTypeSevice : IEnumCommonService
    {
        private readonly IMongoCollection<EnumEntry> collection;
        public EncounterTypeSevice()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            collection = MongoDatabase.GetCollection<EnumEntry>("encountertypes") as IMongoCollection<EnumEntry>;
        }
        public List<EnumEntry> GetAll()
        {
            return collection.Find(x => !x.IsDeleted).ToList();
        }
    }
}
