using MongoDB.Driver;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class CodeSystemService : ICodeSystemService
    {
        private readonly IMongoCollection<CodeSystem> codeSystem;

        public CodeSystemService()
        {
            IMongoDatabase MongoDatabase = Mongo.MongoDBInstance.Instance.GetDatabase();
            codeSystem = MongoDatabase.GetCollection<CodeSystem>("codingsystems") as IMongoCollection<CodeSystem>;
        }
        public List<CodeSystem> GetAll()
        {
            return codeSystem.AsQueryable().Where(x => true).OrderBy(x=>x.Label).ToList();
        }
    }
}
