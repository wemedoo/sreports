namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalThesaurusUserRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GlobalThesaurusRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GlobalThesaurusUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GlobalThesaurusRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.GlobalThesaurusUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.ThesaurusEntries", "UriClassLink", c => c.String());
            AddColumn("dbo.ThesaurusEntries", "UriClassGUI", c => c.String());
            AddColumn("dbo.ThesaurusEntries", "UriSourceLink", c => c.String());
            AddColumn("dbo.ThesaurusEntries", "UriSourceGUI", c => c.String());
            AddColumn("dbo.GlobalThesaurusUsers", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GlobalThesaurusUserRoles", "UserId", "dbo.GlobalThesaurusUsers");
            DropForeignKey("dbo.GlobalThesaurusUserRoles", "RoleId", "dbo.GlobalThesaurusRoles");
            DropIndex("dbo.GlobalThesaurusUserRoles", new[] { "RoleId" });
            DropIndex("dbo.GlobalThesaurusUserRoles", new[] { "UserId" });
            DropColumn("dbo.GlobalThesaurusUsers", "Status");
            DropColumn("dbo.ThesaurusEntries", "UriSourceGUI");
            DropColumn("dbo.ThesaurusEntries", "UriSourceLink");
            DropColumn("dbo.ThesaurusEntries", "UriClassGUI");
            DropColumn("dbo.ThesaurusEntries", "UriClassLink");
            DropTable("dbo.GlobalThesaurusUserRoles");
            DropTable("dbo.GlobalThesaurusRoles");
        }
    }
}
