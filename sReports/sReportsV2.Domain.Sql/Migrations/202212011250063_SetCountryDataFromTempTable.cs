namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetCountryDataFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            context.SetCountryDataFromTempTable("Addresses");
            context.SetCountryDataFromTempTable("PatientAddresses");
        }
        
        public override void Down()
        {
        }
    }
}
