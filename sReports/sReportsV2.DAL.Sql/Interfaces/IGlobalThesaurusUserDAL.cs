using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IGlobalThesaurusUserDAL
    {
        GlobalThesaurusUser GetByEmail(string email);
        GlobalThesaurusUser GetById(int id);
        bool ExistByEmailAndSource(string email, GlobalUserSource source);
        GlobalThesaurusUser InsertOrUpdate(GlobalThesaurusUser user);
        bool IsValidUser(string username, string password);
        List<GlobalThesaurusUser> GetAll();
    }
}
