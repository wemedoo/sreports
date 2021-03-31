using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.PatientEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.OrganizationEntities
{
    [BsonIgnoreExtraElements]
    public class Organization : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public bool Activity { get; set; }
        public List<string> Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public Address Address { get; set; }
        public string PartOf { get; set; }
        public List<Telecom> Telecom { get; set; }
        public List<string> Ancestors { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public Uri LogoUrl { get; set; }
        public List<IdentifierEntity> Identifiers { get; set; }

        public void SetAncestors(Organization parent)
        {
            if(parent != null)
            {
                this.Ancestors = parent.Ancestors != null ? parent.Ancestors.Select(x => x).ToList() : new List<string>();
                this.Ancestors.Add(parent.Id);
            }
        }
    }
}
