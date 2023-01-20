namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTempCountryTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            context.CreateCountryTempTableAndSaveData("Addresses");
            context.CreateCountryTempTableAndSaveData("PatientAddresses");
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            context.Database.ExecuteSqlCommand("dbo.CountryAddressesTempTable");
            context.Database.ExecuteSqlCommand("dbo.CountryPatientAddressesTempTable");
        }
    }
}
