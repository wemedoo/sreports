namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MedicationAndInstanceDoseUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Medications", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Medications", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Medications", "EntryDatetime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Medications", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.MedicationInstances", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationInstances", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.MedicationInstances", "EntryDatetime", c => c.DateTime(nullable: false));
            AddColumn("dbo.MedicationInstances", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.MedicationDoseInstances", "UnitId", c => c.Int());
            CreateIndex("dbo.MedicationDoseInstances", "UnitId");
            AddForeignKey("dbo.MedicationDoseInstances", "UnitId", "dbo.Units", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MedicationDoseInstances", "UnitId", "dbo.Units");
            DropIndex("dbo.MedicationDoseInstances", new[] { "UnitId" });
            DropColumn("dbo.MedicationDoseInstances", "UnitId");
            DropColumn("dbo.MedicationInstances", "LastUpdate");
            DropColumn("dbo.MedicationInstances", "EntryDatetime");
            DropColumn("dbo.MedicationInstances", "RowVersion");
            DropColumn("dbo.MedicationInstances", "IsDeleted");
            DropColumn("dbo.Medications", "LastUpdate");
            DropColumn("dbo.Medications", "EntryDatetime");
            DropColumn("dbo.Medications", "RowVersion");
            DropColumn("dbo.Medications", "IsDeleted");
        }
    }
}
