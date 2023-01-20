namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MedicationDoseCopyValuesFromIntervalToIntervalId : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE MedicationDoses SET IntervalId = Interval;");
        }
        
        public override void Down()
        {
        }
    }
}
