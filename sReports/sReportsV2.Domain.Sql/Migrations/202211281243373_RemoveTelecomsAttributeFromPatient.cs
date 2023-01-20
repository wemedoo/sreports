namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTelecomsAttributeFromPatient : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Telecoms", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients");
            DropForeignKey("dbo.Telecoms", "Patient_Id", "dbo.Patients");
            DropIndex("dbo.Telecoms", new[] { "SmartOncologyPatient_Id" });
            DropIndex("dbo.Telecoms", new[] { "Patient_Id" });
            DropColumn("dbo.Telecoms", "SmartOncologyPatient_Id");
            DropColumn("dbo.Telecoms", "Patient_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Telecoms", "Patient_Id", c => c.Int());
            AddColumn("dbo.Telecoms", "SmartOncologyPatient_Id", c => c.Int());
            CreateIndex("dbo.Telecoms", "Patient_Id");
            CreateIndex("dbo.Telecoms", "SmartOncologyPatient_Id");
            AddForeignKey("dbo.Telecoms", "Patient_Id", "dbo.Patients", "Id");
            AddForeignKey("dbo.Telecoms", "SmartOncologyPatient_Id", "dbo.SmartOncologyPatients", "Id");
        }
    }
}
