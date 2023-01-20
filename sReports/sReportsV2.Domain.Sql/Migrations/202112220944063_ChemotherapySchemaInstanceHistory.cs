namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChemotherapySchemaInstanceVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChemotherapySchemaInstanceId = c.Int(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        FirstDelayDay = c.Int(nullable: false),
                        DelayFor = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatorId, cascadeDelete: false)
                .ForeignKey("dbo.ChemotherapySchemaInstances", t => t.ChemotherapySchemaInstanceId, cascadeDelete: true)
                .Index(t => t.ChemotherapySchemaInstanceId)
                .Index(t => t.CreatorId);
            
            AddColumn("dbo.ChemotherapySchemaInstances", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChemotherapySchemaInstanceVersions", "ChemotherapySchemaInstanceId", "dbo.ChemotherapySchemaInstances");
            DropForeignKey("dbo.ChemotherapySchemaInstanceVersions", "CreatorId", "dbo.Users");
            DropIndex("dbo.ChemotherapySchemaInstanceVersions", new[] { "CreatorId" });
            DropIndex("dbo.ChemotherapySchemaInstanceVersions", new[] { "ChemotherapySchemaInstanceId" });
            DropColumn("dbo.ChemotherapySchemaInstances", "State");
            DropTable("dbo.ChemotherapySchemaInstanceVersions");
        }
    }
}
