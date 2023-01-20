namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesAndVersioningForTelecoms : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            context.SetSystemVersionedTables("dbo.Telecoms");
            context.SetSystemVersionedTables("dbo.PatientTelecoms");
            context.CreateIndexesOnCommonProperties("dbo.Telecoms");
            context.CreateIndexesOnCommonProperties("dbo.PatientTelecoms");
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            context.DropIndexesOnCommonProperties("dbo.PatientTelecoms");
            context.DropIndexesOnCommonProperties("dbo.Telecoms");
            context.UnsetSystemVersionedTables("dbo.PatientTelecoms");
            context.UnsetSystemVersionedTables("dbo.Telecoms");
        }
    }
}
