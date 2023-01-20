using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PermissionDAL : IPermissionDAL
    {
        private readonly SReportsContext context;
        public PermissionDAL(SReportsContext context)
        {
            this.context = context;
        }
        public int Count()
        {
            return context.Permissions.Count();
        }

        public List<Permission> GetAll()
        {
            return context.Permissions.ToList();
        }

        public List<Permission> InsertMany(List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                context.Permissions.Add(permission);
            }
            context.SaveChanges();

            return permissions;
        }

        public bool HasPermission(string name)
        {
            return context.Permissions.Any(m => m.Name.Equals(name));
        }
    }
}
