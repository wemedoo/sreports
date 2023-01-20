namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AcademicPositionType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AcademicPositionTypes", "User_Id", "dbo.Users");
            DropIndex("dbo.AcademicPositionTypes", new[] { "User_Id" });
            CreateTable(
                "dbo.UserAcademicPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AcademicPositionTypeId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AcademicPositionTypes", t => t.AcademicPositionTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AcademicPositionTypeId);
            
            DropColumn("dbo.AcademicPositionTypes", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AcademicPositionTypes", "User_Id", c => c.Int());
            DropForeignKey("dbo.UserAcademicPositions", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAcademicPositions", "AcademicPositionTypeId", "dbo.AcademicPositionTypes");
            DropIndex("dbo.UserAcademicPositions", new[] { "AcademicPositionTypeId" });
            DropIndex("dbo.UserAcademicPositions", new[] { "UserId" });
            DropTable("dbo.UserAcademicPositions");
            RenameColumn(table: "dbo.UserAcademicPositions", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.AcademicPositionTypes", "User_Id");
            AddForeignKey("dbo.AcademicPositionTypes", "User_Id", "dbo.Users", "Id");
        }
    }
}
