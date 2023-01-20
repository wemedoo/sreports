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
    public class GlobalThesaurusUserDAL : IGlobalThesaurusUserDAL
    {
        private SReportsContext context;
        public GlobalThesaurusUserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool ExistByEmailAndSource(string email, GlobalUserSource source)
        {
            return context.GlobalUser.Any(x => x.Email.Equals(email) && x.Source == source);
        }

        public GlobalThesaurusUser GetByEmail(string email)
        {
            return context.GlobalUser
                .Include("Roles")
                .Include("Roles.Role")
                .FirstOrDefault(x => x.Email.Equals(email));
        }

        public GlobalThesaurusUser GetById(int id)
        {
            return context.GlobalUser
                .Include("Roles")
                .Include("Roles.Role")
                .FirstOrDefault(x => x.GlobalThesaurusUserId == id);
        }

        public List<GlobalThesaurusUser> GetAll()
        {
            return context.GlobalUser
                .Include("Roles")
                .Include("Roles.Role")
                .Where(u => !u.IsDeleted)
                .ToList();
        }

        public GlobalThesaurusUser InsertOrUpdate(GlobalThesaurusUser user)
        {
            if (user.GlobalThesaurusUserId == 0)
            {
                context.GlobalUser.Add(user);
            }

            context.SaveChanges();

            return user;
        }

        public bool IsValidUser(string username, string password)
        {
            return context.GlobalUser.Any(x => x.Email.Equals(username) && x.Password.Equals(password) && x.Status == GlobalUserStatus.Active);
        }
    }
}
