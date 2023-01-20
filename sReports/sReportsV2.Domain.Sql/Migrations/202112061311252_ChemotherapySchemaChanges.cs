namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChemotherapySchemaInstances", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "Patient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.Indications", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.MedicationDoses", "Medication_Id", "dbo.Medications");
            DropForeignKey("dbo.MedicationDoseInstances", "MedicationInstance_Id", "dbo.MedicationInstances");
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "ChemotherapySchema_Id" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "Creator_Id" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "Patient_Id" });
            DropIndex("dbo.Indications", new[] { "ChemotherapySchema_Id" });
            DropIndex("dbo.MedicationDoses", new[] { "Medication_Id" });
            DropIndex("dbo.MedicationDoseInstances", new[] { "MedicationInstance_Id" });
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "ChemotherapySchema_Id", newName: "ChemotherapySchemaId");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "Creator_Id", newName: "CreatorId");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "Patient_Id", newName: "PatientId");
            RenameColumn(table: "dbo.Indications", name: "ChemotherapySchema_Id", newName: "ChemotherapySchemaId");
            RenameColumn(table: "dbo.MedicationDoses", name: "Medication_Id", newName: "MedicationId");
            RenameColumn(table: "dbo.MedicationDoseInstances", name: "MedicationInstance_Id", newName: "MedicationInstanceId");
            AddColumn("dbo.Indications", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Medications", "UnitId", c => c.Int());
            AddColumn("dbo.Medications", "CumulativeDoseUnitId", c => c.Int());
            AddColumn("dbo.MedicationDoses", "IntervalId", c => c.Int(nullable: false));
            AddColumn("dbo.MedicationDoses", "UnitId", c => c.Int());
            AlterColumn("dbo.ChemotherapySchemaInstances", "ChemotherapySchemaId", c => c.Int(nullable: false));
            AlterColumn("dbo.ChemotherapySchemaInstances", "CreatorId", c => c.Int(nullable: false));
            AlterColumn("dbo.ChemotherapySchemaInstances", "PatientId", c => c.Int(nullable: false));
            AlterColumn("dbo.Indications", "ChemotherapySchemaId", c => c.Int(nullable: false));
            AlterColumn("dbo.MedicationDoses", "MedicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.MedicationDoseInstances", "MedicationInstanceId", c => c.Int(nullable: false));
            CreateIndex("dbo.ChemotherapySchemaInstances", "PatientId");
            CreateIndex("dbo.ChemotherapySchemaInstances", "CreatorId");
            CreateIndex("dbo.ChemotherapySchemaInstances", "ChemotherapySchemaId");
            CreateIndex("dbo.Indications", "ChemotherapySchemaId");
            CreateIndex("dbo.Medications", "UnitId");
            CreateIndex("dbo.Medications", "CumulativeDoseUnitId");
            CreateIndex("dbo.MedicationDoses", "UnitId");
            CreateIndex("dbo.MedicationDoses", "MedicationId");
            CreateIndex("dbo.MedicationDoseInstances", "MedicationInstanceId");
            AddForeignKey("dbo.Medications", "CumulativeDoseUnitId", "dbo.Units", "Id");
            AddForeignKey("dbo.MedicationDoses", "UnitId", "dbo.Units", "Id");
            AddForeignKey("dbo.Medications", "UnitId", "dbo.Units", "Id");
            AddForeignKey("dbo.ChemotherapySchemaInstances", "ChemotherapySchemaId", "dbo.ChemotherapySchemas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChemotherapySchemaInstances", "PatientId", "dbo.SmartOncologyPatients", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Indications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MedicationDoses", "MedicationId", "dbo.Medications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MedicationDoseInstances", "MedicationInstanceId", "dbo.MedicationInstances", "Id", cascadeDelete: true);
            DropColumn("dbo.Medications", "Unit");
            DropColumn("dbo.Medications", "CumulativeDoseUnit");
            DropColumn("dbo.MedicationDoses", "StartsAt");
            DropColumn("dbo.MedicationDoses", "Unit");
            DropColumn("dbo.MedicationDoseTypes", "StartAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MedicationDoseTypes", "StartAt", c => c.String());
            AddColumn("dbo.MedicationDoses", "Unit", c => c.String());
            AddColumn("dbo.MedicationDoses", "StartsAt", c => c.String());
            AddColumn("dbo.Medications", "CumulativeDoseUnit", c => c.String());
            AddColumn("dbo.Medications", "Unit", c => c.String());
            DropForeignKey("dbo.MedicationDoseInstances", "MedicationInstanceId", "dbo.MedicationInstances");
            DropForeignKey("dbo.MedicationDoses", "MedicationId", "dbo.Medications");
            DropForeignKey("dbo.Indications", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "PatientId", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "ChemotherapySchemaId", "dbo.ChemotherapySchemas");
            DropForeignKey("dbo.Medications", "UnitId", "dbo.Units");
            DropForeignKey("dbo.MedicationDoses", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Medications", "CumulativeDoseUnitId", "dbo.Units");
            DropIndex("dbo.MedicationDoseInstances", new[] { "MedicationInstanceId" });
            DropIndex("dbo.MedicationDoses", new[] { "MedicationId" });
            DropIndex("dbo.MedicationDoses", new[] { "UnitId" });
            DropIndex("dbo.Medications", new[] { "CumulativeDoseUnitId" });
            DropIndex("dbo.Medications", new[] { "UnitId" });
            DropIndex("dbo.Indications", new[] { "ChemotherapySchemaId" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "ChemotherapySchemaId" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "CreatorId" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "PatientId" });
            AlterColumn("dbo.MedicationDoseInstances", "MedicationInstanceId", c => c.Int());
            AlterColumn("dbo.MedicationDoses", "MedicationId", c => c.Int());
            AlterColumn("dbo.Indications", "ChemotherapySchemaId", c => c.Int());
            AlterColumn("dbo.ChemotherapySchemaInstances", "PatientId", c => c.Int());
            AlterColumn("dbo.ChemotherapySchemaInstances", "CreatorId", c => c.Int());
            AlterColumn("dbo.ChemotherapySchemaInstances", "ChemotherapySchemaId", c => c.Int());
            DropColumn("dbo.MedicationDoses", "UnitId");
            DropColumn("dbo.MedicationDoses", "IntervalId");
            DropColumn("dbo.Medications", "CumulativeDoseUnitId");
            DropColumn("dbo.Medications", "UnitId");
            DropColumn("dbo.Indications", "IsDeleted");
            RenameColumn(table: "dbo.MedicationDoseInstances", name: "MedicationInstanceId", newName: "MedicationInstance_Id");
            RenameColumn(table: "dbo.MedicationDoses", name: "MedicationId", newName: "Medication_Id");
            RenameColumn(table: "dbo.Indications", name: "ChemotherapySchemaId", newName: "ChemotherapySchema_Id");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "PatientId", newName: "Patient_Id");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "CreatorId", newName: "Creator_Id");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "ChemotherapySchemaId", newName: "ChemotherapySchema_Id");
            CreateIndex("dbo.MedicationDoseInstances", "MedicationInstance_Id");
            CreateIndex("dbo.MedicationDoses", "Medication_Id");
            CreateIndex("dbo.Indications", "ChemotherapySchema_Id");
            CreateIndex("dbo.ChemotherapySchemaInstances", "Patient_Id");
            CreateIndex("dbo.ChemotherapySchemaInstances", "Creator_Id");
            CreateIndex("dbo.ChemotherapySchemaInstances", "ChemotherapySchema_Id");
            AddForeignKey("dbo.MedicationDoseInstances", "MedicationInstance_Id", "dbo.MedicationInstances", "Id");
            AddForeignKey("dbo.MedicationDoses", "Medication_Id", "dbo.Medications", "Id");
            AddForeignKey("dbo.Indications", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas", "Id");
            AddForeignKey("dbo.ChemotherapySchemaInstances", "Patient_Id", "dbo.SmartOncologyPatients", "Id");
            AddForeignKey("dbo.ChemotherapySchemaInstances", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas", "Id");
        }
    }
}
