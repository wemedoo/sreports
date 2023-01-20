namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOnlyOnePatientAddress : DbMigration
    {
        public override void Up()
        {
            string removeAddress = @"ALTER TABLE [dbo].[Patients] DROP CONSTRAINT IF EXISTS [FK_dbo.Patients_dbo.Addresses_Addresss_Id]; " +
                "ALTER TABLE [dbo].[SmartOncologyPatients] DROP CONSTRAINT IF EXISTS [FK_dbo.SmartOncologyPatients_dbo.Addresses_Address_Id]; " +
                "DROP INDEX IF EXISTS dbo.Patients.IX_Address_Id; " +
                "DROP INDEX IF EXISTS dbo.SmartOncologyPatients.IX_Address_Id; " +
                "ALTER TABLE [dbo].[SmartOncologyPatients] DROP COLUMN IF EXISTS Address_Id; " +
                "DELETE FROM dbo.Addresses WHERE Id IN (select Address_Id from dbo.Patients); " +
                "UPDATE dbo.Patients SET Address_Id = NULL; " +
                "ALTER TABLE [dbo].[Patients] DROP COLUMN IF EXISTS Address_Id;";
            Sql(removeAddress);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Patients", "Address_Id", c => c.Int());
            AddColumn("dbo.SmartOncologyPatients", "Address_Id", c => c.Int());
            CreateIndex("dbo.Patients", "Address_Id");
            CreateIndex("dbo.SmartOncologyPatients", "Address_Id");
            AddForeignKey("dbo.Patients", "Address_Id", "dbo.Addresses", "Id");
            AddForeignKey("dbo.SmartOncologyPatients", "Address_Id", "dbo.Addresses", "Id");
        }
    }
}
