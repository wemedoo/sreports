namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSaltProperty : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Users", "Salt", c => c.String(maxLength: 50));
        }

        public override void Down()
        {
            //DropColumn("dbo.Users", "Salt");
        }
    }
}
