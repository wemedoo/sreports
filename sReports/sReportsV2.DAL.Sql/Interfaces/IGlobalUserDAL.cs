using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IGlobalUserDAL
    {
        GlobalThesaurusUser GetByEmail(string email);
        bool ExistByEmailAndSource(string email, GlobalUserSource source);
        void InsertOrUpdate(GlobalThesaurusUser user);
        bool IsValidUser(string username, string password);
    }
}
