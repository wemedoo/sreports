using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.AccessManagment
{
    public class Role : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("RoleId")]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<PermissionRole> Permissions { get; set; } = new List<PermissionRole>();

        public void Copy(Role role)
        {
            this.Name = role.Name;
            this.Description = role.Description;
        }

        public bool PermissionsHaveChanged(Role role)
        {
            return !Permissions.SequenceEqual(role.Permissions, new PermissionRoleComparer());
        }
    }
}
