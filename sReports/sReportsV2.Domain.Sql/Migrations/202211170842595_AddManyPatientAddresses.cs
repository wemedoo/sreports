namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyPatientAddresses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        City = c.String(maxLength: 100),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 10),
                        Country = c.String(maxLength: 50),
                        Street = c.String(maxLength: 200),
                        StreetNumber = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatientAddresses", "PatientId", "dbo.Patients");
            DropIndex("dbo.PatientAddresses", new[] { "PatientId" });
            DropTable("dbo.PatientAddresses");
        }
    }
}
