using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IGlobalThesaurusRoleDAL
    {
        int Count();
        List<GlobalThesaurusRole> GetAll();

        GlobalThesaurusRole GetById(int id);

        GlobalThesaurusRole GetByName(string roleName);

        void InsertOrUpdate(GlobalThesaurusRole role);
    }
}
