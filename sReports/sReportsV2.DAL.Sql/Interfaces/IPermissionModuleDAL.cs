using sReportsV2.Domain.Sql.Entities.AccessManagment;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPermissionModuleDAL
    {
        void InsertMany(List<PermissionModule> permissionModules);
        List<PermissionModule> GetAllByModule(string moduleName);

    }
}
