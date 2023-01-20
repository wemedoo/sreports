namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThesaurusMerge_MigrateToSql : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThesaurusMerges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewThesaurus = c.Int(nullable: false),
                        OldThesaurus = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        CompletedCollectionsString = c.String(),
                        FailedCollectionsString = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntryDatetime = c.DateTime(nullable: false),
                        LastUpdate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ThesaurusMerges");
        }
    }
}
