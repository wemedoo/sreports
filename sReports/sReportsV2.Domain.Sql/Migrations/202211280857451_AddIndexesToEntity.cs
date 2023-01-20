namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesToEntity : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();

            context.CreateIndexesOnCommonProperties("dbo.Addresses");
            context.CreateIndexesOnCommonProperties("dbo.ChemotherapySchemaInstances");
            context.CreateIndexesOnCommonProperties("dbo.ChemotherapySchemas");
            context.CreateIndexesOnCommonProperties("dbo.Users");
            context.CreateIndexesOnCommonProperties("dbo.UserAcademicPositions");
            context.CreateIndexesOnCommonProperties("dbo.Organizations");
            context.CreateIndexesOnCommonProperties("dbo.OrganizationClinicalDomains");
            context.CreateIndexesOnCommonProperties("dbo.OrganizationRelations");
            context.CreateIndexesOnCommonProperties("dbo.UserRoles");
            context.CreateIndexesOnCommonProperties("dbo.Roles");
            context.CreateIndexesOnCommonProperties("dbo.Medications");
            context.CreateIndexesOnCommonProperties("dbo.ChemotherapySchemaInstanceVersions");
            context.CreateIndexesOnCommonProperties("dbo.MedicationReplacements");
            context.CreateIndexesOnCommonProperties("dbo.MedicationInstances");
            context.CreateIndexesOnCommonProperties("dbo.SmartOncologyPatients");
            context.CreateIndexesOnCommonProperties("dbo.EpisodeOfCares");
            context.CreateIndexesOnCommonProperties("dbo.Comments");
            context.CreateIndexesOnCommonProperties("dbo.CustomEnums");
            context.CreateIndexesOnCommonProperties("dbo.ThesaurusEntries");
            context.CreateIndexesOnCommonProperties("dbo.Encounters");
            context.CreateIndexesOnCommonProperties("dbo.GlobalThesaurusRoles");
            context.CreateIndexesOnCommonProperties("dbo.GlobalThesaurusUserRoles");
            context.CreateIndexesOnCommonProperties("dbo.GlobalThesaurusUsers");
            context.CreateIndexesOnCommonProperties("dbo.OutsideUsers");
            context.CreateIndexesOnCommonProperties("dbo.Patients");
            context.CreateIndexesOnCommonProperties("dbo.PatientAddresses");
            context.CreateIndexesOnCommonProperties("dbo.ThesaurusMerges");
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();

            context.DropIndexesOnCommonProperties("dbo.ThesaurusMerges");
            context.DropIndexesOnCommonProperties("dbo.PatientAddresses");
            context.DropIndexesOnCommonProperties("dbo.Patients");
            context.DropIndexesOnCommonProperties("dbo.OutsideUsers");
            context.DropIndexesOnCommonProperties("dbo.GlobalThesaurusUsers");
            context.DropIndexesOnCommonProperties("dbo.GlobalThesaurusUserRoles");
            context.DropIndexesOnCommonProperties("dbo.GlobalThesaurusRoles");
            context.DropIndexesOnCommonProperties("dbo.Encounters");
            context.DropIndexesOnCommonProperties("dbo.ThesaurusEntries");
            context.DropIndexesOnCommonProperties("dbo.CustomEnums");
            context.DropIndexesOnCommonProperties("dbo.Comments");
            context.DropIndexesOnCommonProperties("dbo.EpisodeOfCares");
            context.DropIndexesOnCommonProperties("dbo.SmartOncologyPatients");
            context.DropIndexesOnCommonProperties("dbo.MedicationInstances");
            context.DropIndexesOnCommonProperties("dbo.MedicationReplacements");
            context.DropIndexesOnCommonProperties("dbo.ChemotherapySchemaInstanceVersions");
            context.DropIndexesOnCommonProperties("dbo.Medications");
            context.DropIndexesOnCommonProperties("dbo.Roles");
            context.DropIndexesOnCommonProperties("dbo.UserRoles");
            context.DropIndexesOnCommonProperties("dbo.OrganizationRelations");
            context.DropIndexesOnCommonProperties("dbo.OrganizationClinicalDomains");
            context.DropIndexesOnCommonProperties("dbo.Organizations");
            context.DropIndexesOnCommonProperties("dbo.UserAcademicPositions");
            context.DropIndexesOnCommonProperties("dbo.Users");
            context.DropIndexesOnCommonProperties("dbo.ChemotherapySchemas");
            context.DropIndexesOnCommonProperties("dbo.ChemotherapySchemaInstances");
            context.DropIndexesOnCommonProperties("dbo.Addresses");
        }
    }
}
