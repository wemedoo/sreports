namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDanglingIndications : DbMigration
    {
        public override void Up()
        {
            Sql("delete FROM Indications where ChemotherapySchema_Id IS NULL; ");
        }

        public override void Down()
        {
        }
    }
}
