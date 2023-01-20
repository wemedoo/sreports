namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MedicationDoses", "MedicationId", "dbo.Medications");
            DropIndex("dbo.MedicationDoses", new[] { "MedicationId" });
            RenameColumn(table: "dbo.MedicationDoses", name: "MedicationId", newName: "Medication_Id");
            CreateTable(
                "dbo.ChemotherapySchemaInstances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                        ChemotherapySchema_Id = c.Int(),
                        Creator_Id = c.Int(),
                        Patient_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChemotherapySchemas", t => t.ChemotherapySchema_Id)
                .ForeignKey("dbo.Users", t => t.Creator_Id)
                .ForeignKey("dbo.SmartOncologyPatients", t => t.Patient_Id)
                .Index(t => t.ChemotherapySchema_Id)
                .Index(t => t.Creator_Id)
                .Index(t => t.Patient_Id);
            
            CreateTable(
                "dbo.MedicationInstances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Medication = c.Int(nullable: false),
                        ChemotherapySchemaInstance_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChemotherapySchemaInstances", t => t.ChemotherapySchemaInstance_Id)
                .Index(t => t.ChemotherapySchemaInstance_Id);
            
            CreateTable(
                "dbo.MedicationDoseInstances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DayNumber = c.Int(nullable: false),
                        Date = c.DateTime(),
                        MedicationDose = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        MedicationInstance_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MedicationInstances", t => t.MedicationInstance_Id)
                .Index(t => t.MedicationInstance_Id);
            
            CreateTable(
                "dbo.MedicationDoseTimeInstances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time = c.String(),
                        Dose = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        MedicationDoseInstance_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MedicationDoseInstances", t => t.MedicationDoseInstance_Id)
                .Index(t => t.MedicationDoseInstance_Id);
            
            AlterColumn("dbo.MedicationDoses", "Medication_Id", c => c.Int());
            CreateIndex("dbo.MedicationDoses", "Medication_Id");
            AddForeignKey("dbo.MedicationDoses", "Medication_Id", "dbo.Medications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedicationDoses", "Medication_Id", "dbo.Medications");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "Patient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.MedicationInstances", "ChemotherapySchemaInstance_Id", "dbo.ChemotherapySchemaInstances");
            DropForeignKey("dbo.MedicationDoseInstances", "MedicationInstance_Id", "dbo.MedicationInstances");
            DropForeignKey("dbo.MedicationDoseTimeInstances", "MedicationDoseInstance_Id", "dbo.MedicationDoseInstances");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "Creator_Id", "dbo.Users");
            DropForeignKey("dbo.ChemotherapySchemaInstances", "ChemotherapySchema_Id", "dbo.ChemotherapySchemas");
            DropIndex("dbo.MedicationDoseTimeInstances", new[] { "MedicationDoseInstance_Id" });
            DropIndex("dbo.MedicationDoseInstances", new[] { "MedicationInstance_Id" });
            DropIndex("dbo.MedicationInstances", new[] { "ChemotherapySchemaInstance_Id" });
            DropIndex("dbo.MedicationDoses", new[] { "Medication_Id" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "Patient_Id" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "Creator_Id" });
            DropIndex("dbo.ChemotherapySchemaInstances", new[] { "ChemotherapySchema_Id" });
            AlterColumn("dbo.MedicationDoses", "Medication_Id", c => c.Int(nullable: false));
            DropTable("dbo.MedicationDoseTimeInstances");
            DropTable("dbo.MedicationDoseInstances");
            DropTable("dbo.MedicationInstances");
            DropTable("dbo.ChemotherapySchemaInstances");
            RenameColumn(table: "dbo.MedicationDoses", name: "Medication_Id", newName: "MedicationId");
            CreateIndex("dbo.MedicationDoses", "MedicationId");
            AddForeignKey("dbo.MedicationDoses", "MedicationId", "dbo.Medications", "Id", cascadeDelete: true);
        }
    }
}
