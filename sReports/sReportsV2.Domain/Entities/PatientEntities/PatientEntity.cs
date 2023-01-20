using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    [BsonIgnoreExtraElements]
    public class PatientEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<Identifier> Identifiers { get; set; }
        public bool Active { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? BirthDate { get; set; }
        public Address Addresss { get; set; }
        public MultipleBirth MultipleB { get; set; }
        public Contact ContactPerson { get; set; }
        public List<Telecom> Telecoms { get; set; }
        public List<Communication> Communications { get; set; }



        public void SetGenderFromString(string gender)
        {
            switch (gender)
            {
                case "Male":
                    this.Gender = Gender.Male; break;
                case "Female":
                    this.Gender = Gender.Female; break;
                case "Other":
                    this.Gender = Gender.Other; break;
                case "Unknown":
                    this.Gender = Gender.Unknown; break;
                default:
                    this.Gender = Gender.Unknown; break;
            }
        }

    }
}
