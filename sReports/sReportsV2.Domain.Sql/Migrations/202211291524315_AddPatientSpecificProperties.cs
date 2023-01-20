namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientSpecificProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "CitizenshipId", c => c.Int());
            AddColumn("dbo.Patients", "ReligionId", c => c.Int());
            AddColumn("dbo.Patients", "DeceasedDateTime", c => c.DateTime());
            AddColumn("dbo.Patients", "Deceased", c => c.Boolean());
            CreateIndex("dbo.Patients", "CitizenshipId");
            CreateIndex("dbo.Patients", "ReligionId");
            AddForeignKey("dbo.Patients", "CitizenshipId", "dbo.CustomEnums", "Id");
            AddForeignKey("dbo.Patients", "ReligionId", "dbo.CustomEnums", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "ReligionId", "dbo.CustomEnums");
            DropForeignKey("dbo.Patients", "CitizenshipId", "dbo.CustomEnums");
            DropIndex("dbo.Patients", new[] { "ReligionId" });
            DropIndex("dbo.Patients", new[] { "CitizenshipId" });
            DropColumn("dbo.Patients", "Deceased");
            DropColumn("dbo.Patients", "DeceasedDateTime");
            DropColumn("dbo.Patients", "ReligionId");
            DropColumn("dbo.Patients", "CitizenshipId");
        }
    }
}
