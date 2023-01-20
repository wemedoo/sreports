namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityParentToAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Addresses", "EntryDatetime", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
            AddColumn("dbo.Addresses", "LastUpdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "LastUpdate");
            DropColumn("dbo.Addresses", "EntryDatetime");
            DropColumn("dbo.Addresses", "RowVersion");
            DropColumn("dbo.Addresses", "IsDeleted");
        }
    }
}
