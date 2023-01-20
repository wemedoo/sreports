using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class RoleDAL : IRoleDAL
    {
        private readonly SReportsContext context;

        public RoleDAL(SReportsContext context)
        {
            this.context = context;
        }

        public int Count()
        {
            return context.Roles.Count();
        }

        public List<Role> GetAll()
        {
            return context.Roles.ToList();
        }

        public Role GetById(int id)
        {
            return context.Roles.FirstOrDefault(x => x.RoleId == id);
        }

        public Role GetByName(string roleName)
        {
            return context.Roles.FirstOrDefault(x => x.Name == roleName);
        }

        public void InsertOrUpdate(Role role)
        {
            if(role.RoleId == 0)
            {
                context.Roles.Add(role);
            }
            else
            {
                role.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public List<Role> GetAll(RoleFilter roleFilter)
        {
            IQueryable<Role> result = GetAll().AsQueryable<Role>();

            if (roleFilter.ColumnName != null)
            {
                result = SortTableHelper.OrderByField(result, roleFilter.ColumnName, roleFilter.IsAscending)
                     .Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                     .Take(roleFilter.PageSize);
            }
            else
            {
                result = result.Skip((roleFilter.Page - 1) * roleFilter.PageSize)
                     .Take(roleFilter.PageSize);
            }

            return result.ToList();
        }

        public long GetAllFilteredCount()
        {
            return GetAll().Count();
        }
    }
}
