namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceMedications : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Medications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropIndex("dbo.Medications", new[] { "ChemotherapySchemaId" });
            CreateTable(
                "dbo.MedicationReplacements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChemotherapySchemaInstanceId = c.Int(nullable: false),
                        ReplaceMedicationId = c.Int(nullable: false),
                        ReplaceWithMedicationId = c.Int(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatorId, cascadeDelete: false)
                .ForeignKey("dbo.MedicationInstances", t => t.ReplaceMedicationId, cascadeDelete: false)
                .ForeignKey("dbo.MedicationInstances", t => t.ReplaceWithMedicationId, cascadeDelete: false)
                .ForeignKey("dbo.ChemotherapySchemaInstances", t => t.ChemotherapySchemaInstanceId, cascadeDelete: false)
                .Index(t => t.ChemotherapySchemaInstanceId)
                .Index(t => t.ReplaceMedicationId)
                .Index(t => t.ReplaceWithMedicationId)
                .Index(t => t.CreatorId);
            
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "Description", c => c.String());
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "ActionType", c => c.Int(nullable: false));
            AlterColumn("dbo.Medications", "ChemotherapySchemaId", c => c.Int());
            CreateIndex("dbo.Medications", "ChemotherapySchemaId");
            AddForeignKey("dbo.Medications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Medications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.MedicationReplacements", "ChemotherapySchemaInstanceId", "dbo.ChemotherapySchemaInstances");
            DropForeignKey("dbo.MedicationReplacements", "ReplaceWithMedicationId", "dbo.MedicationInstances");
            DropForeignKey("dbo.MedicationReplacements", "ReplaceMedicationId", "dbo.MedicationInstances");
            DropForeignKey("dbo.MedicationReplacements", "CreatorId", "dbo.Users");
            DropIndex("dbo.MedicationReplacements", new[] { "CreatorId" });
            DropIndex("dbo.MedicationReplacements", new[] { "ReplaceWithMedicationId" });
            DropIndex("dbo.MedicationReplacements", new[] { "ReplaceMedicationId" });
            DropIndex("dbo.MedicationReplacements", new[] { "ChemotherapySchemaInstanceId" });
            DropIndex("dbo.Medications", new[] { "ChemotherapySchemaId" });
            AlterColumn("dbo.Medications", "ChemotherapySchemaId", c => c.Int(nullable: false));
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "ActionType");
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "Description");
            DropTable("dbo.MedicationReplacements");
            CreateIndex("dbo.Medications", "ChemotherapySchemaId");
            AddForeignKey("dbo.Medications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas", "Id", cascadeDelete: true);
        }
    }
}
