namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccessManagment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Roles", "User_Id", "dbo.Users");
            DropIndex("dbo.Roles", new[] { "User_Id" });
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermissionModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .Index(t => t.ModuleId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermissionRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.Roles", "Description", c => c.String());
            AddColumn("dbo.Roles", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Roles", "EntryDatetime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Roles", "LastUpdate", c => c.DateTime());
            DropColumn("dbo.Roles", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "User_Id", c => c.Int());
            DropForeignKey("dbo.PermissionRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.PermissionModules", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.PermissionModules", "ModuleId", "dbo.Modules");
            DropIndex("dbo.PermissionRoles", new[] { "RoleId" });
            DropIndex("dbo.PermissionModules", new[] { "PermissionId" });
            DropIndex("dbo.PermissionModules", new[] { "ModuleId" });
            DropColumn("dbo.Roles", "LastUpdate");
            DropColumn("dbo.Roles", "EntryDatetime");
            DropColumn("dbo.Roles", "RowVersion");
            DropColumn("dbo.Roles", "IsDeleted");
            DropColumn("dbo.Roles", "Description");
            DropTable("dbo.PermissionRoles");
            DropTable("dbo.Permissions");
            DropTable("dbo.PermissionModules");
            DropTable("dbo.Modules");
            CreateIndex("dbo.Roles", "User_Id");
            AddForeignKey("dbo.Roles", "User_Id", "dbo.Users", "Id");
        }
    }
}
