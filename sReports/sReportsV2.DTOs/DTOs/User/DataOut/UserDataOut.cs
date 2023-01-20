using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
        public string PersonalEmail { get; set; }
        public string ContactPhone { get; set; }
        public UserPrefix Prefix { get; set; }
        public AddressDTO Address{ get; set; }
        public byte[] RowVersion { get; set; }
        public List<RoleDataOut> Roles { get; set; } = new List<RoleDataOut>();
        public List<AcademicPosition> AcademicPositions { get; set; } = new List<AcademicPosition>();
        public List<UserOrganizationDataOut> Organizations { get; set; }
        public List<ClinicalTrialDTO> ClinicalTrials { get; set; }

        public bool IsBlockedInEveryOrganization()
        {
            return Organizations.Count > 0 && Organizations.All(x => x.State == UserState.Blocked);
        }

        public bool HasToChooseActiveOrganization(int activeOrganizationId)
        {
            return Organizations.Count == 0 || GetUserStateInOrganization(activeOrganizationId) != UserState.Active;
        }

        public List<int> GetOrganizationRefs()
        {
            return this.Organizations.Select(x => x.OrganizationId).ToList();
        }

        public override string ToString()
        {
            return $"{Username} ({FirstName} {LastName})";
        }

        public string GetorganizationListFormatted()
        {
            return string.Join(", ", Organizations.Select(x => x.Organization.Name));
        }

        public IEnumerable<UserOrganizationDataOut> GetActiveOrganizations()
        {
            return Organizations.Where(x => x.State == UserState.Active);
        }

        public IEnumerable<UserOrganizationDataOut> GetNonArchivedOrganizations()
        {
            return Organizations.Where(x => x.State != UserState.Archived);
        }

        public bool IsUserBlocked(int activeOrganizationId)
        {
            return GetUserStateInOrganization(activeOrganizationId) == UserState.Blocked;
        }

        public List<Claim> GetClaims() 
        {
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Email, this.Email),
                new Claim(ClaimTypes.Name, this.Username),
                new Claim("preferred_username", this.Email)
            };

            return claims;
        }

        public bool IsRoleChecked(int roleId)
        {
            return Roles.Any(r => r.Id == roleId);
        }

        public string RenderUserRoles()
        {
            return string.Join(", ", Roles.Select(x => x.Name).ToArray());
        }

        public List<string> RenderUserRoleNames()
        {
            return Roles.Select(x => x.Name).ToList();
        }

        private UserState? GetUserStateInOrganization(int organizationId)
        {
            return Organizations?.FirstOrDefault(x => x.OrganizationId == organizationId)?.State;
        }
    }
}