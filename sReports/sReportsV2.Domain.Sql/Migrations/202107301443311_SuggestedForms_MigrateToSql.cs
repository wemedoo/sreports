namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuggestedForms_MigrateToSql : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserConfigs", "SuggestedFormsString", c => c.String());
            AddColumn("dbo.UserConfigs", "PredefinedFormsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserConfigs", "PredefinedFormsString");
            DropColumn("dbo.UserConfigs", "SuggestedFormsString");
        }
    }
}
