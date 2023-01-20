namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceHistoryComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "ReasonForDelay", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "ReasonForDelay");
        }
    }
}
