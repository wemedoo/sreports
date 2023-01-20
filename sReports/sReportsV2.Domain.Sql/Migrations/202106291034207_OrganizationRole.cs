namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganizationRole : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Roles", "UserOrganization_Id", "dbo.UserOrganizations");
            DropIndex("dbo.Roles", new[] { "UserOrganization_Id" });
            CreateTable(
                "dbo.OrganizationRoleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrganizationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        UserOrganization_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganizationRoleTypes", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.UserOrganizations", t => t.UserOrganization_Id)
                .Index(t => t.RoleId)
                .Index(t => t.UserOrganization_Id);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "UserOrganization_Id", c => c.Int());
            DropForeignKey("dbo.OrganizationRoles", "UserOrganization_Id", "dbo.UserOrganizations");
            DropForeignKey("dbo.OrganizationRoles", "RoleId", "dbo.OrganizationRoleTypes");
            DropIndex("dbo.OrganizationRoles", new[] { "UserOrganization_Id" });
            DropIndex("dbo.OrganizationRoles", new[] { "RoleId" });
            DropTable("dbo.OrganizationRoles");
            DropTable("dbo.OrganizationRoleTypes");
            CreateIndex("dbo.Roles", "UserOrganization_Id");
            AddForeignKey("dbo.Roles", "UserOrganization_Id", "dbo.UserOrganizations", "Id");
        }
    }
}
