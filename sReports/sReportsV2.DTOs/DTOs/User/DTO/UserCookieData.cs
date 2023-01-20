using Newtonsoft.Json;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DTO
{
    public class UserCookieData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveLanguage { get; set; }
        public int ActiveOrganization { get; set; }
        public int PageSize { get; set; }
        public string Email { get; set; }
        public List<RoleDataOut> Roles { get; set; }
        public List<OrganizationDataOut> Organizations { get; set; }
        public List<string> SuggestedForms { get; set; }

        public string TimeZoneOffset { get; set; }
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
                this.SuggestedForms = result.SuggestedForms;
            }

        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public OrganizationDataOut GetActiveOrganizationData()
        {
            return Organizations.FirstOrDefault(x => x.Id.Equals(ActiveOrganization));
        }

        public string GetActiveOrganizationName()
        {
            return GetActiveOrganizationData()?.Name;
        }

        public bool UserHasPermission(string permissionName, string moduleName)
        {
            return Roles
                .SelectMany(r => r.Permissions)
                .Any(p => p.ModuleName.Equals(moduleName) && p.Permission.Name.Equals(permissionName));
        }

        public bool UserHasAnyOfRole(params string[] roleNames)
        {
            return Roles.Any(r => roleNames.Any(v => v == r.Name));
        }
    }
}