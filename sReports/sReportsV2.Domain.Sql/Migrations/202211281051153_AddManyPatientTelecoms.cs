namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyPatientTelecoms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientTelecoms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        System = c.String(),
                        Value = c.String(),
                        Use = c.String(),
                        Active = c.Boolean(nullable: false),
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
            DropForeignKey("dbo.PatientTelecoms", "PatientId", "dbo.Patients");
            DropIndex("dbo.PatientTelecoms", new[] { "PatientId" });
            DropTable("dbo.PatientTelecoms");
        }
    }
}
