namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChemotherapySchemaInstanceCopyValues : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM MedicationInstances WHERE Medication = 0; UPDATE MedicationInstances SET MedicationId = Medication;");
        }

        public override void Down()
        {
        }
    }
}
