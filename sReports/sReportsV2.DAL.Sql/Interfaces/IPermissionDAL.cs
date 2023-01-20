using sReportsV2.Domain.Sql.Entities.AccessManagment;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPermissionDAL
    {
        int Count();
        List<Permission> GetAll();
        List<Permission> InsertMany(List<Permission> permissions);
        bool HasPermission(string permissionName);

    }
}
