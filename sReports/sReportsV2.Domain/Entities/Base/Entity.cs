using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Exceptions;
using System;

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
                throw new ConcurrencyException();
            }
        }

        public void DoConcurrencyBeforeDeleteCheck(DateTime lastUpdate)
        {
            if (this.LastUpdate != lastUpdate.ToUniversalTime())
            {
                throw new ConcurrencyDeleteEditException();
            }
        }

        public static void DoConcurrencyCheckForDelete(Entity forDelete)
        {
            if (forDelete == null)
            {
                throw new ConcurrencyDeleteException();
            }
        }

        public void Copy(Entity entity)
        {
            this.EntryDatetime = entity == null ? DateTime.Now : entity.EntryDatetime;
            SetLastUpdate();
        }

        public void SetLastUpdate()
        {
            this.LastUpdate = DateTime.Now;
        }
    }
}
