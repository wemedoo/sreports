namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddressType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "AddressTypeId", c => c.Int());
            AddColumn("dbo.PatientAddresses", "AddressTypeId", c => c.Int());
            CreateIndex("dbo.Addresses", "AddressTypeId");
            CreateIndex("dbo.PatientAddresses", "AddressTypeId");
            AddForeignKey("dbo.Addresses", "AddressTypeId", "dbo.CustomEnums", "Id");
            AddForeignKey("dbo.PatientAddresses", "AddressTypeId", "dbo.CustomEnums", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientAddresses", "AddressTypeId", "dbo.CustomEnums");
            DropForeignKey("dbo.Addresses", "AddressTypeId", "dbo.CustomEnums");
            DropIndex("dbo.PatientAddresses", new[] { "AddressTypeId" });
            DropIndex("dbo.Addresses", new[] { "AddressTypeId" });
            DropColumn("dbo.PatientAddresses", "AddressTypeId");
            DropColumn("dbo.Addresses", "AddressTypeId");
        }
    }
}
