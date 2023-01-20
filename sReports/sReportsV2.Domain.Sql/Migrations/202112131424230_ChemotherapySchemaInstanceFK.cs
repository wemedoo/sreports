namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceFK : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MedicationInstances", "MedicationId");
            AddForeignKey("dbo.MedicationInstances", "MedicationId", "dbo.Medications", "Id", cascadeDelete: false);
            DropColumn("dbo.MedicationInstances", "Medication");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MedicationInstances", "Medication", c => c.Int(nullable: false));
            DropForeignKey("dbo.MedicationInstances", "MedicationId", "dbo.Medications");
            DropIndex("dbo.MedicationInstances", new[] { "MedicationId" });
        }
    }
}
