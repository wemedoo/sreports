namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Helpers;
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.User;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class PasswordHash : DbMigration
    {
        public override void Up()
        {
            SReportsContext sReportsContext = new SReportsContext();
            if (sReportsContext.User.Any())
            {
                foreach (User user in sReportsContext.User)
                {
                    if (string.IsNullOrWhiteSpace(user.Salt))
                    {
                        string salt = PasswordHelper.CreateSalt(10);
                        user.Salt = salt;
                        user.Password = PasswordHelper.Hash(user.Password, salt);
                    }

                }
                sReportsContext.SaveChanges();
            }
        }
        
        public override void Down()
        {
        }
    }
}
