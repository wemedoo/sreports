using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class OutsideUser : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string InstitutionAddress { get; set; }
        public Address Address { get; set; }

    }
}
