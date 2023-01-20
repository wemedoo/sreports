using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.AccessManagment
{
    public class PermissionRole
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PermissionRoleId")]
        public int PermissionRoleId { get; set; }
        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
        public int RoleId { get; set; }
    }

    public class PermissionRoleComparer : IEqualityComparer<PermissionRole>
    {
        public bool Equals(PermissionRole x, PermissionRole y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ModuleId == y.ModuleId && x.PermissionId == y.PermissionId;
        }

        public int GetHashCode(PermissionRole permissionRole)
        {
            if (Object.ReferenceEquals(permissionRole, null)) return 0;
            
            int hashModuleId = permissionRole.ModuleId.GetHashCode();
            int hashPermissionId = permissionRole.PermissionId.GetHashCode();

            return hashModuleId ^ hashPermissionId;
        }
    }
}
