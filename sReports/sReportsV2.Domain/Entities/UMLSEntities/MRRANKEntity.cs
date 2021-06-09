using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.UMLSEntities
{
    public class MRRANKEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Rank { get; set; }
        public string Sab { get; set; }
        public string TTY { get; set; }
        public string SupPress { get; set; }

        public MRRANKEntity() { }
        public MRRANKEntity(string rank, string sab, string tty, string supPress) 
        {
            this.Rank = rank;
            this.Sab = sab;
            this.TTY = tty;
            this.SupPress = supPress;
            this.EntryDatetime = DateTime.Now;
            this.LastUpdate = DateTime.Now;
            this.IsDeleted = false;

        }

    }
}
