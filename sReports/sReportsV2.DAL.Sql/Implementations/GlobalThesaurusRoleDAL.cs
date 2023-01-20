using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class GlobalThesaurusRoleDAL : IGlobalThesaurusRoleDAL
    {
        private readonly SReportsContext context;

        public GlobalThesaurusRoleDAL(SReportsContext context)
        {
            this.context = context;
        }
        public int Count()
        {
            return context.GlobalThesaurusRoles.Count();
        }

        public List<GlobalThesaurusRole> GetAll()
        {
            return context.GlobalThesaurusRoles.OrderBy(r => r.GlobalThesaurusRoleId).ToList();
        }

        public GlobalThesaurusRole GetById(int id)
        {
            return context.GlobalThesaurusRoles.FirstOrDefault(x => x.GlobalThesaurusRoleId == id);
        }

        public GlobalThesaurusRole GetByName(string roleName)
        {
            return context.GlobalThesaurusRoles.FirstOrDefault(x => x.Name == roleName);
        }

        public void InsertOrUpdate(GlobalThesaurusRole role)
        {
            if (role.GlobalThesaurusRoleId == 0)
            {
                context.GlobalThesaurusRoles.Add(role);
            }
            else
            {
                role.SetLastUpdate();
            }
            context.SaveChanges();
        }
    }
}
