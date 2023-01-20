using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser
{
    public class GlobalThesaurusUser : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("GlobalThesaurusUserId")]
        public int GlobalThesaurusUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public GlobalUserSource? Source { get; set; }
        public string Affiliation { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public GlobalUserStatus Status { get; set; }
        public virtual List<GlobalThesaurusUserRole> Roles { get; set; } = new List<GlobalThesaurusUserRole>();

        public void UpdateRoles(List<int> newSelectedRoles)
        {
            if (RolesHaveChanged(newSelectedRoles))
            {
                AddUserRoles(newSelectedRoles);
                RemoveUserRoles(newSelectedRoles);
            }
        }

        public bool HasRole(string roleName)
        {
            return Roles
                .Where(r => !r.IsDeleted)
                .Select(r => r.Role)
                .Any(r => r.Name == roleName);
        }

        private bool RolesHaveChanged(List<int> newRoles)
        {
            return !newRoles.SequenceEqual(GetRolesIds());
        }

        private List<int> GetRolesIds()
        {
            return Roles.Where(x => !x.IsDeleted).Select(x => x.RoleId).ToList();
        }

        private void AddUserRoles(List<int> newRoles)
        {
            if (newRoles != null)
            {
                foreach (var userRoleId in newRoles)
                {
                    GlobalThesaurusUserRole userRole = Roles.FirstOrDefault(x => x.RoleId == userRoleId && !x.IsDeleted);
                    if (userRole == null)
                    {
                        Roles.Add(new GlobalThesaurusUserRole()
                        {
                            UserId = GlobalThesaurusUserId,
                            RoleId = userRoleId
                        });
                    }
                }
            }
        }

        private void RemoveUserRoles(List<int> newRoles)
        {
            foreach (GlobalThesaurusUserRole userRole in Roles)
            {
                int roleId = newRoles.FirstOrDefault(x => x == userRole.RoleId);
                if (roleId == 0)
                {
                    userRole.IsDeleted = true;
                    userRole.SetLastUpdate();
                }
            }
        }

    }
}
