using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.User.DTO;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace sReportsV2.DTOs.Common.DataOut
{
    public class UserDataOut
    {
        public int Id { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public UserPrefix Prefix { get; set; }
        public AddressDTO Address{ get; set; }
        public byte[] RowVersion { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public List<AcademicPosition> AcademicPositions { get; set; }
        public List<UserOrganizationDataOut> Organizations { get; set; }
        public List<ClinicalTrialDTO> ClinicalTrials { get; set; }

        public List<int> GetOrganizationRefs()
        {
            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }

        public override string ToString()
        {
            return $"{Username} ({FirstName} {LastName})";
        }

        public List<RoleDTO> GetRolesByOrganizationId(int organizationId) 
        {
            return this.Organizations != null
                && this.Organizations.Count > 0
                && this.Organizations.FirstOrDefault(x => x.OrganizationId == organizationId) != null
                ? this.Organizations.FirstOrDefault(x => x.OrganizationId == organizationId).Roles : new List<RoleDTO>();
        }

        public string GetorganizationListFormatted()
        {
            string result = string.Empty;
            for (int i = 0; i < this.Organizations.Count; i++)
            {
                result += this.Organizations != null ? this.Organizations[i].Organization.Name : "";
                if (i < this.Organizations.Count - 1)
                {
                    result += "<text> , </text>";
                }
            }
            return result;
        }

        public UserState? GetStateByOrganizationId(int organizationId)
        {
            return Organizations?.FirstOrDefault(x => x.OrganizationId == organizationId).State;
        }

        public List<Claim> GetClaims() 
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, this.Email));
            claims.Add(new Claim(ClaimTypes.Name, this.Username));
            claims.Add(new Claim("preferred_username", this.Email));

            if (this.Roles != null)
            {
                foreach (var role in this.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            return claims;
        }
    }
}