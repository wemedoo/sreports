using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities
{
    public class Entity
    {
        public bool IsDeleted { get; set; }

        [BsonDateTimeOptions(Representation = BsonType.Document)]
        public DateTime EntryDatetime { get; set; }

        [BsonDateTimeOptions(Representation = BsonType.Document)]
        public DateTime? LastUpdate { get; set; }

        public void DoConcurrencyCheck(DateTime lastUpdate) 
        {
            if (this.LastUpdate != lastUpdate.ToUniversalTime())
            {
                throw new MongoDbConcurrencyException();
            }
        }

        public static void DoConcurrencyCheckForDelete(Entity forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }
    }
}
