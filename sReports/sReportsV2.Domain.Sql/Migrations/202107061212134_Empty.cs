namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Empty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ThesaurusEntries", "PreferredLanguage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ThesaurusEntries", "PreferredLanguage");
        }
    }
}
