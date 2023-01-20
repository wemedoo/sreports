namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonalEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "PersonalEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "PersonalEmail");
        }
    }
}
