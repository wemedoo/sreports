namespace sReportsV2.DTOs.DTOs.AccessManagment.DataOut
{
    public class PermissionRoleDataOut
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int PermissionId { get; set; }
        public virtual PermissionDataOut Permission { get; set; }
    }
}
