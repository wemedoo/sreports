namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitImpressum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organizations", "Impressum", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "Impressum");
        }
    }
}
