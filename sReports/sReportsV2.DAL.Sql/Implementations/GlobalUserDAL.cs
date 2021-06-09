using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Common.Enums;

namespace sReportsV2.SqlDomain.Implementations
{
    public class GlobalUserDAL : IGlobalUserDAL
    {
        private SReportsContext context;
        public GlobalUserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool ExistByEmailAndSource(string email, GlobalUserSource source)
        {
            return context.GlobalUser.Any(x => x.Email.Equals(email) && x.Source == source);
        }

        public GlobalThesaurusUser GetByEmail(string email)
        {
            return context.GlobalUser.FirstOrDefault(x => x.Email.Equals(email));
        }

        public void InsertOrUpdate(GlobalThesaurusUser user)
        {
            if (user.Id == 0)
            {
                user.EntryDatetime = DateTime.Now;
                context.GlobalUser.Add(user);
            }

            context.SaveChanges();
        }

        public bool IsValidUser(string username, string password)
        {
            return context.GlobalUser.Any(x => x.Email.Equals(username) && x.Password.Equals(password));
        }
    }
}
