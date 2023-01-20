namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MedicationInstances", "ChemotherapySchemaInstance_Id", "dbo.ChemotherapySchemaInstances");
            DropForeignKey("dbo.MedicationDoseTimeInstances", "MedicationDoseInstance_Id", "dbo.MedicationDoseInstances");
            DropIndex("dbo.MedicationInstances", new[] { "ChemotherapySchemaInstance_Id" });
            DropIndex("dbo.MedicationDoseTimeInstances", new[] { "MedicationDoseInstance_Id" });
            RenameColumn(table: "dbo.MedicationInstances", name: "ChemotherapySchemaInstance_Id", newName: "ChemotherapySchemaInstanceId");
            RenameColumn(table: "dbo.MedicationDoseTimeInstances", name: "MedicationDoseInstance_Id", newName: "MedicationDoseInstanceId");
            AddColumn("dbo.MedicationInstances", "MedicationId", c => c.Int(nullable: false));
            AddColumn("dbo.MedicationDoseInstances", "IntervalId", c => c.Int());
            AlterColumn("dbo.MedicationInstances", "ChemotherapySchemaInstanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.MedicationDoseTimeInstances", "MedicationDoseInstanceId", c => c.Int(nullable: false));
            CreateIndex("dbo.MedicationInstances", "ChemotherapySchemaInstanceId");
            CreateIndex("dbo.MedicationDoseTimeInstances", "MedicationDoseInstanceId");
            AddForeignKey("dbo.MedicationInstances", "ChemotherapySchemaInstanceId", "dbo.ChemotherapySchemaInstances", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MedicationDoseTimeInstances", "MedicationDoseInstanceId", "dbo.MedicationDoseInstances", "Id", cascadeDelete: true);
            DropColumn("dbo.ChemotherapySchemaInstances", "Name");
            DropColumn("dbo.MedicationDoseInstances", "MedicationDose");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MedicationDoseInstances", "MedicationDose", c => c.Int(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstances", "Name", c => c.String());
            DropForeignKey("dbo.MedicationDoseTimeInstances", "MedicationDoseInstanceId", "dbo.MedicationDoseInstances");
            DropForeignKey("dbo.MedicationInstances", "ChemotherapySchemaInstanceId", "dbo.ChemotherapySchemaInstances");
            DropIndex("dbo.MedicationDoseTimeInstances", new[] { "MedicationDoseInstanceId" });
            DropIndex("dbo.MedicationInstances", new[] { "ChemotherapySchemaInstanceId" });
            AlterColumn("dbo.MedicationDoseTimeInstances", "MedicationDoseInstanceId", c => c.Int());
            AlterColumn("dbo.MedicationInstances", "ChemotherapySchemaInstanceId", c => c.Int());
            DropColumn("dbo.MedicationDoseInstances", "IntervalId");
            DropColumn("dbo.MedicationInstances", "MedicationId");
            RenameColumn(table: "dbo.MedicationDoseTimeInstances", name: "MedicationDoseInstanceId", newName: "MedicationDoseInstance_Id");
            RenameColumn(table: "dbo.MedicationInstances", name: "ChemotherapySchemaInstanceId", newName: "ChemotherapySchemaInstance_Id");
            CreateIndex("dbo.MedicationDoseTimeInstances", "MedicationDoseInstance_Id");
            CreateIndex("dbo.MedicationInstances", "ChemotherapySchemaInstance_Id");
            AddForeignKey("dbo.MedicationDoseTimeInstances", "MedicationDoseInstance_Id", "dbo.MedicationDoseInstances", "Id");
            AddForeignKey("dbo.MedicationInstances", "ChemotherapySchemaInstance_Id", "dbo.ChemotherapySchemaInstances", "Id");
        }
    }
}
