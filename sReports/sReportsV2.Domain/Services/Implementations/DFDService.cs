using MongoDB.Driver;
using sReportsV2.Domain.Entities.DFD;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class DFDService : IDFDService
    {

        //private readonly MongoHelper<Form> Forms;
        private readonly IMongoCollection<DFDFormInfo> Collection;
        public DFDService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<DFDFormInfo>("dfdforminfo") as IMongoCollection<DFDFormInfo>;
        }
        public List<DFDFormInfo> GetAll()
        {
            return Collection.AsQueryable().ToList();
        }

        public void Insert(DFDFormInfo formInfo)
        {
            formInfo.Copy(null);
          
            Collection.InsertOne(formInfo);
        }

        
    }
}
