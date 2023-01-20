namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmartOncologyConsecutiveChemotherapyDays : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SmartOncologyPatients", "ConsecutiveChemotherapyDays", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SmartOncologyPatients", "ConsecutiveChemotherapyDays", c => c.String());
        }
    }
}
