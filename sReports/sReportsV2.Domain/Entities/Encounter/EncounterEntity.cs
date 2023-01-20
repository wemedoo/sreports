using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Encounter
{
    // ---------------------------- NOT USED ANYMORE --------------------------
    public class EncounterEntity: Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string EpisodeOfCareId { get; set; }
        public int Status { get; set; }
        public int Class { get; set; }
        public int Type { get; set; }
        public int ServiceType { get; set; }
        public PeriodDatetime Period { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }
        [BsonIgnore]
        public List<FormInstance.FormInstance> FormInstances { get; set; }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.Status = this.Status == oldThesaurus ? newThesaurus : this.Status;
            this.Class = this.Class == oldThesaurus ? newThesaurus : this.Class;
            this.Type = this.Type == oldThesaurus ? newThesaurus : this.Type;
            this.ServiceType = this.ServiceType == oldThesaurus ? newThesaurus : this.ServiceType;
        }

    }
}
