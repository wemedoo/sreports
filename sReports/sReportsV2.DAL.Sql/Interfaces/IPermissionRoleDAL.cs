using sReportsV2.Domain.Sql.Entities.AccessManagment;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPermissionRoleDAL
    {
        void DeletePermissionsForRole(int roleId);
    }
}
