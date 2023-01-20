namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MedicationDoseRemoveIntervalColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MedicationDoses", "Interval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MedicationDoses", "Interval", c => c.String());
        }
    }
}
