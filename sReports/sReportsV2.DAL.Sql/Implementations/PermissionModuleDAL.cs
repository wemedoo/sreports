using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PermissionModuleDAL : IPermissionModuleDAL
    {
        private readonly SReportsContext context;
        public PermissionModuleDAL(SReportsContext context)
        {
            this.context = context;
        }

        public List<PermissionModule> GetAllByModule(string moduleName)
        {
            return context.PermissionModules.Where(x => x.Module.Name.Equals(moduleName)).ToList();
        }

        public void InsertMany(List<PermissionModule> permissionModules)
        {
            foreach (var permissionModule in permissionModules)
            {
                context.PermissionModules.Add(permissionModule);
            }
            context.SaveChanges();
        }
    }
}
