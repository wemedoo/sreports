namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityParentToTelecoms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Telecoms", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Telecoms", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Telecoms", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("dbo.Telecoms", "EntryDatetime", c => c.DateTime(nullable: false, defaultValueSql: "getdate()"));
            AddColumn("dbo.Telecoms", "LastUpdate", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.Telecoms", "LastUpdate");
            DropColumn("dbo.Telecoms", "EntryDatetime");
            DropColumn("dbo.Telecoms", "RowVersion");
            DropColumn("dbo.Telecoms", "IsDeleted");
            DropColumn("dbo.Telecoms", "Active");
        }
    }
}
