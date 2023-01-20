using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class OutsideUserService : IOutsideUserService
    {
        private readonly IMongoCollection<OutsideUser> Collection;
        public OutsideUserService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<OutsideUser>("outsideuser");
        }

        public bool Delete(string id)
        {
            OutsideUser forDelete = Collection.AsQueryable().FirstOrDefault(x => x.Id == id);
            var filter = Builders<OutsideUser>.Filter.Eq(x => x.Id, id);
            var update = Builders<OutsideUser>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);

            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }


        public List<OutsideUser> GetAllByIds(List<int> ids)
        {
            return Collection.AsQueryable().Where(x => ids.Contains(Int32.Parse(x.Id))).ToList();
        }

        public OutsideUser GetById(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id == id);
        }

        public string InsertOrUpdate(OutsideUser user)
        {
            user = Ensure.IsNotNull(user, nameof(user));


            if (user.Id == null)
            {
                user.Copy(null);
                Collection.InsertOne(user);
            }
            else
            {
                OutsideUser userForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(user.Id));
                user.Copy(userForUpdate);
                FilterDefinition<OutsideUser> filter = Builders<OutsideUser>.Filter.Eq(s => s.Id, user.Id);
                var result = Collection.ReplaceOne(filter, user).ModifiedCount;
            }

            return user.Id;

        }
    }
}
