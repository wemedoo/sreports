namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SavePatientAddressData : DbMigration
    {
        public override void Up()
        {
            string insertCommand = "insert into dbo.PatientAddresses (PatientId, City, State, PostalCode, Country, Street, StreetNumber, IsDeleted, EntryDatetime) " +
                "select p.Id, a.City, a.State, a.PostalCode, a.Country, a.Street, a.StreetNumber, p.IsDeleted, p.EntryDatetime " +
                "from dbo.Patients p inner join dbo.Addresses a on p.Address_Id = a.Id;";
            Sql(insertCommand);
        }
        
        public override void Down()
        {
            
        }
    }
}
