namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Organization_Role : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.OrganizationRoleTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OrganizationRoleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
