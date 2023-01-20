using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PermissionRoleDAL : IPermissionRoleDAL
    {
        private readonly SReportsContext context;
        public PermissionRoleDAL(SReportsContext context)
        {
            this.context = context;
        }
        public void DeletePermissionsForRole(int roleId)
        {
            IQueryable<PermissionRole> permissionsToRemove = context.PermissionRoles.Where(p => p.RoleId == roleId);
            context.PermissionRoles.RemoveRange(permissionsToRemove);
            context.SaveChanges();
        }
    }
}
