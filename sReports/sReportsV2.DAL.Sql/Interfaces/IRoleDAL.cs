using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using System;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IRoleDAL
    {
        void InsertOrUpdate(Role role);
        int Count();
        List<Role> GetAll();
        Role GetById(int id);
        Role GetByName(string roleName);
        List<Role> GetAll(RoleFilter roleFilter);
        long GetAllFilteredCount();
    }
}
