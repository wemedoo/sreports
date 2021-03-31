using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    [BsonIgnoreExtraElements]
    public class PatientEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<IdentifierEntity> Identifiers { get; set; }
        public bool Active { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Address Addresss { get; set; }
        public MultipleBirth MultipleB { get; set; }
        public Contact ContactPerson { get; set; }
        public List<Telecom> Telecoms { get; set; }
        public List<Communication> Communications { get; set; }

        [BsonIgnore]
        public List<EpisodeOfCareEntities.EpisodeOfCareEntity> EpisodeOfCares { get; set; }


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
