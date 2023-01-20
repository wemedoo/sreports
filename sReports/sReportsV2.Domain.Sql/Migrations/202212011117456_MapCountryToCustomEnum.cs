namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MapCountryToCustomEnum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "CountryId", c => c.Int());
            AddColumn("dbo.PatientAddresses", "CountryId", c => c.Int());
            CreateIndex("dbo.Addresses", "CountryId");
            CreateIndex("dbo.PatientAddresses", "CountryId");
            AddForeignKey("dbo.Addresses", "CountryId", "dbo.CustomEnums", "Id");
            AddForeignKey("dbo.PatientAddresses", "CountryId", "dbo.CustomEnums", "Id");
            DropColumn("dbo.Addresses", "Country");
            DropColumn("dbo.PatientAddresses", "Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PatientAddresses", "Country", c => c.String(maxLength: 50));
            AddColumn("dbo.Addresses", "Country", c => c.String(maxLength: 50));
            DropForeignKey("dbo.PatientAddresses", "CountryId", "dbo.CustomEnums");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.CustomEnums");
            DropIndex("dbo.PatientAddresses", new[] { "CountryId" });
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropColumn("dbo.PatientAddresses", "CountryId");
            DropColumn("dbo.Addresses", "CountryId");
        }
    }
}
