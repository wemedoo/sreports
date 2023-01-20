namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntervalIdNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MedicationDoses", "IntervalId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MedicationDoses", "IntervalId", c => c.Int(nullable: false));
        }
    }
}
