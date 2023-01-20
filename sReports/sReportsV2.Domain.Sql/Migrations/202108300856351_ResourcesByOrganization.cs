namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourcesByOrganization : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "ActiveOrganizationId", "dbo.Organizations");
            DropIndex("dbo.Users", new[] { "ActiveOrganizationId" });
            AddColumn("dbo.Patients", "OrganizationId", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "ActiveOrganizationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "ActiveOrganizationId", c => c.Int());
            DropColumn("dbo.Patients", "OrganizationId");
            CreateIndex("dbo.Users", "ActiveOrganizationId");
            AddForeignKey("dbo.Users", "ActiveOrganizationId", "dbo.Organizations", "Id");
        }
    }
}
