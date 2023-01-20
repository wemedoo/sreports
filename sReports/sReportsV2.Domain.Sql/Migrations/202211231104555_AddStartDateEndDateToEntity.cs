namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddStartDateEndDateToEntity : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();

            context.SetSystemVersionedTables("dbo.Addresses");
            context.SetSystemVersionedTables("dbo.ChemotherapySchemaInstances");
            context.SetSystemVersionedTables("dbo.ChemotherapySchemas");
            context.SetSystemVersionedTables("dbo.Users");
            context.SetSystemVersionedTables("dbo.UserAcademicPositions");
            context.SetSystemVersionedTables("dbo.Organizations");
            context.SetSystemVersionedTables("dbo.OrganizationClinicalDomains");
            context.SetSystemVersionedTables("dbo.OrganizationRelations");
            context.SetSystemVersionedTables("dbo.UserRoles");
            context.SetSystemVersionedTables("dbo.Roles");
            context.SetSystemVersionedTables("dbo.Medications");
            context.SetSystemVersionedTables("dbo.ChemotherapySchemaInstanceVersions");
            context.SetSystemVersionedTables("dbo.MedicationReplacements");
            context.SetSystemVersionedTables("dbo.MedicationInstances");
            context.SetSystemVersionedTables("dbo.SmartOncologyPatients");
            context.SetSystemVersionedTables("dbo.EpisodeOfCares");
            context.SetSystemVersionedTables("dbo.Comments");
            context.SetSystemVersionedTables("dbo.CustomEnums");
            context.SetSystemVersionedTables("dbo.ThesaurusEntries");
            context.SetSystemVersionedTables("dbo.Encounters");
            context.SetSystemVersionedTables("dbo.GlobalThesaurusRoles");
            context.SetSystemVersionedTables("dbo.GlobalThesaurusUserRoles");
            context.SetSystemVersionedTables("dbo.GlobalThesaurusUsers");
            context.SetSystemVersionedTables("dbo.OutsideUsers");
            context.SetSystemVersionedTables("dbo.Patients");
            context.SetSystemVersionedTables("dbo.PatientAddresses");
            context.SetSystemVersionedTables("dbo.ThesaurusMerges");
        }

        public override void Down()
        {
            SReportsContext context = new SReportsContext();

            context.UnsetSystemVersionedTables("dbo.ThesaurusMerges");
            context.UnsetSystemVersionedTables("dbo.PatientAddresses");
            context.UnsetSystemVersionedTables("dbo.Patients");
            context.UnsetSystemVersionedTables("dbo.OutsideUsers");
            context.UnsetSystemVersionedTables("dbo.GlobalThesaurusUsers");
            context.UnsetSystemVersionedTables("dbo.GlobalThesaurusUserRoles");
            context.UnsetSystemVersionedTables("dbo.GlobalThesaurusRoles");
            context.UnsetSystemVersionedTables("dbo.Encounters");
            context.UnsetSystemVersionedTables("dbo.ThesaurusEntries");
            context.UnsetSystemVersionedTables("dbo.CustomEnums");
            context.UnsetSystemVersionedTables("dbo.Comments");
            context.UnsetSystemVersionedTables("dbo.EpisodeOfCares");
            context.UnsetSystemVersionedTables("dbo.SmartOncologyPatients");
            context.UnsetSystemVersionedTables("dbo.MedicationInstances");
            context.UnsetSystemVersionedTables("dbo.MedicationReplacements");
            context.UnsetSystemVersionedTables("dbo.ChemotherapySchemaInstanceVersions");
            context.UnsetSystemVersionedTables("dbo.Medications");
            context.UnsetSystemVersionedTables("dbo.Roles");
            context.UnsetSystemVersionedTables("dbo.UserRoles");
            context.UnsetSystemVersionedTables("dbo.OrganizationRelations");
            context.UnsetSystemVersionedTables("dbo.OrganizationClinicalDomains");
            context.UnsetSystemVersionedTables("dbo.Organizations");
            context.UnsetSystemVersionedTables("dbo.UserAcademicPositions");
            context.UnsetSystemVersionedTables("dbo.Users");
            context.UnsetSystemVersionedTables("dbo.ChemotherapySchemas");
            context.UnsetSystemVersionedTables("dbo.ChemotherapySchemaInstances");
            context.UnsetSystemVersionedTables("dbo.Addresses");
        }
    }
}
