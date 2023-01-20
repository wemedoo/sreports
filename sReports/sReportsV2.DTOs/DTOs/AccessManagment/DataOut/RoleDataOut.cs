using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataOut
{
    public class RoleDataOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionRoleDataOut> Permissions { get; set; } = new List<PermissionRoleDataOut>();

        public bool IsPermissionChecked(int moduleId, int permissionId)
        {
            return Permissions.Any(p => p.ModuleId == moduleId && p.PermissionId == permissionId);
        }
    }
}
