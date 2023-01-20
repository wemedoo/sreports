namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivePropertyToEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstances", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemas", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserAcademicPositions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationClinicalDomains", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganizationRelations", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserRoles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Medications", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChemotherapySchemaInstanceVersions", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationReplacements", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.MedicationInstances", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.EpisodeOfCares", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomEnums", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThesaurusEntries", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.Encounters", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusRoles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUserRoles", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.GlobalThesaurusUsers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutsideUsers", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientAddresses", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.ThesaurusMerges", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ThesaurusMerges", "Active");
            DropColumn("dbo.PatientAddresses", "Active");
            DropColumn("dbo.OutsideUsers", "Active");
            DropColumn("dbo.GlobalThesaurusUsers", "Active");
            DropColumn("dbo.GlobalThesaurusUserRoles", "Active");
            DropColumn("dbo.GlobalThesaurusRoles", "Active");
            DropColumn("dbo.Encounters", "Active");
            DropColumn("dbo.ThesaurusEntries", "Active");
            DropColumn("dbo.CustomEnums", "Active");
            DropColumn("dbo.Comments", "Active");
            DropColumn("dbo.EpisodeOfCares", "Active");
            DropColumn("dbo.MedicationInstances", "Active");
            DropColumn("dbo.MedicationReplacements", "Active");
            DropColumn("dbo.ChemotherapySchemaInstanceVersions", "Active");
            DropColumn("dbo.Medications", "Active");
            DropColumn("dbo.Roles", "Active");
            DropColumn("dbo.UserRoles", "Active");
            DropColumn("dbo.OrganizationRelations", "Active");
            DropColumn("dbo.OrganizationClinicalDomains", "Active");
            DropColumn("dbo.Organizations", "Active");
            DropColumn("dbo.UserAcademicPositions", "Active");
            DropColumn("dbo.Users", "Active");
            DropColumn("dbo.ChemotherapySchemas", "Active");
            DropColumn("dbo.ChemotherapySchemaInstances", "Active");
            DropColumn("dbo.Addresses", "Active");
        }
    }
}
