namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SavePatientTelecomData : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string insertPatientTelecoms = @"insert into dbo.PatientTelecoms (System, Value, [Use], PatientId, Active, IsDeleted, EntryDatetime) select System
              ,Value
              ,[Use]
              ,Patient_Id
              ,t.Active
              ,t.IsDeleted
              ,t.EntryDatetime
              from dbo.Telecoms t
              inner join dbo.Patients p
              on p.Id = t.Patient_Id;";

            context.Database.ExecuteSqlCommand(insertPatientTelecoms);
        }
        
        public override void Down()
        {
        }
    }
}
