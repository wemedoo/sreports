using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.AccessManagment.DataIn
{
    public class RoleDataIn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionRoleDataIn> Permissions { get; set; } = new List<PermissionRoleDataIn>();

    }
}
