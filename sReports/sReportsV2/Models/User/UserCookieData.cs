using MongoDB.Bson;
using Newtonsoft.Json;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.User
{
    public class UserCookieData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveLanguage { get; set; }
        public string ActiveOrganization { get; set; }
        public int PageSize { get; set; }
        public List<string> Roles { get; set; }
        public List<OrganizationDataOut> Organizations { get; set; }

        public UserCookieData()
        {

        }

        public UserCookieData (string json)
        {
                if (!string.IsNullOrEmpty(json))
            {
                UserCookieData result = JsonConvert.DeserializeObject<UserCookieData>(json);
                this.FirstName = result.FirstName;
                this.LastName = result.LastName;
                this.ActiveLanguage = result.ActiveLanguage;
                this.ActiveOrganization = result.ActiveOrganization;
                this.Username = result.Username;
                this.Organizations = result.Organizations;
                this.Id = result.Id;
                this.Roles = result.Roles;
            }

        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public OrganizationDataOut GetActiveOrganizationData()
        {
            return Organizations.FirstOrDefault(x => x.Id.Equals(ActiveOrganization)) ?? throw new NullReferenceException();
        }
    }
}