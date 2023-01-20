using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut
{
    public class GlobalThesaurusUserDataOut
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public GlobalUserSource? Source { get; set; }
        public string Affiliation { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public GlobalUserStatus Status { get; set; }
        public virtual List<RoleDataOut> Roles { get; set; } = new List<RoleDataOut>();

        public List<Claim> GetClaims()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Name, Email)
            };

            if (this.Roles != null)
            {
                foreach (var role in this.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            return claims;
        }

        public string RenderUserRoles()
        {
            string roles = string.Join(", ", Roles.Select(x => x.Name).ToArray());
            return string.IsNullOrWhiteSpace(roles) ? "No roles" : roles;
        }

        public bool IsRoleChecked(int roleId)
        {
            return Roles.Any(r => r.Id == roleId);
        }
    }
}
