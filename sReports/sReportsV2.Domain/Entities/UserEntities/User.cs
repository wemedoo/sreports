using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.UserEntities
{
    [BsonIgnoreExtraElements]
    public class User : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string ActiveLanguage { get; set; }
        public int PageSize { get; set; } = 5;
        public string ActiveOrganization { get; set; }
        public List<string> OrganizationRefs { get; set; }
        public List<string> Roles { get; set; }
    }
}
