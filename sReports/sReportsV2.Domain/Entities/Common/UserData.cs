using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    [BsonIgnoreExtraElements]
    public class UserData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveOrganization { get; set; }
        public List<string> OrganizationRefs { get; set; }
    }
}
