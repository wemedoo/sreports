namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
    using System;
    using System.Data.Entity.Migrations;

    public partial class DbNamingConvention : DbMigration
    {
        public override void Up()
        {
            // User
            RenameColumn(table: "dbo.Users", name: "Id", newName: "UserId");

            RenameColumn(table: "dbo.UserAcademicPositions", name: "Id", newName: "UserAcademicPositionId");
            RenameColumn(table: "dbo.AcademicPositionTypes", name: "Id", newName: "AcademicPositionTypeId");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "Id", newName: "UserClinicalTrialId");
            RenameColumn(table: "dbo.UserOrganizations", name: "Id", newName: "UserOrganizationId");
            RenameColumn(table: "dbo.UserConfigs", name: "Id", newName: "UserConfigId");
            RenameColumn(table: "dbo.UserRoles", name: "Id", newName: "UserRoleId");

            // AccessManagement
            RenameColumn(table: "dbo.Roles", name: "Id", newName: "RoleId");
            RenameColumn(table: "dbo.PermissionRoles", name: "Id", newName: "PermissionRoleId");
            RenameColumn(table: "dbo.Modules", name: "Id", newName: "ModuleId");
            RenameColumn(table: "dbo.PermissionModules", name: "Id", newName: "PermissionModuleId");
            RenameColumn(table: "dbo.Permissions", name: "Id", newName: "PermissionId");

            // ChemotherapySchema
            RenameColumn(table: "dbo.BodySurfaceCalculationFormulas", name: "Id", newName: "BodySurfaceCalculationFormulaId");
            RenameColumn(table: "dbo.ChemotherapySchemas", name: "Id", newName: "ChemotherapySchemaId");
            RenameColumn(table: "dbo.Indications", name: "Id", newName: "IndicationId");
            RenameColumn(table: "dbo.LiteratureReferences", name: "Id", newName: "LiteratureReferenceId");
            RenameColumn(table: "dbo.Medications", name: "Id", newName: "MedicationId");
            RenameColumn(table: "dbo.Units", name: "Id", newName: "UnitId");
            RenameColumn(table: "dbo.MedicationDoses", name: "Id", newName: "MedicationDoseId");
            RenameColumn(table: "dbo.MedicationDoseTimes", name: "Id", newName: "MedicationDoseTimeId");
            RenameColumn(table: "dbo.MedicationDoseTypes", name: "Id", newName: "MedicationDoseTypeId");
            RenameColumn(table: "dbo.RouteOfAdministrations", name: "Id", newName: "RouteOfAdministrationId");

            //ChemotherapySchemaInstance
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "Id", newName: "ChemotherapySchemaInstanceId");
            RenameColumn(table: "dbo.ChemotherapySchemaInstanceVersions", name: "Id", newName: "ChemotherapySchemaInstanceVersionId");
            RenameColumn(table: "dbo.MedicationReplacements", name: "Id", newName: "MedicationReplacementId");
            RenameColumn(table: "dbo.MedicationInstances", name: "Id", newName: "MedicationInstanceId");
            RenameColumn(table: "dbo.MedicationDoseInstances", name: "Id", newName: "MedicationDoseInstanceId");
            RenameColumn(table: "dbo.MedicationDoseTimeInstances", name: "Id", newName: "MedicationDoseTimeInstanceId");

            // CodeSystem
            RenameColumn(table: "dbo.CodeSystems", name: "Id", newName: "CodeSystemId");

            // Common
            RenameColumn(table: "dbo.Addresses", name: "Id", newName: "AddressId");
            RenameColumn(table: "dbo.CustomEnums", name: "Id", newName: "CustomEnumId");
            RenameColumn(table: "dbo.Identifiers", name: "Id", newName: "IdentifierId");
            RenameColumn(table: "dbo.Telecoms", name: "Id", newName: "TelecomId");

            RenameColumn(table: "dbo.OutsideUsers", name: "Address_Id", newName: "AddressId");
            RenameColumn(table: "dbo.Contacts", name: "Address_Id", newName: "AddressId");

            // Encounter
            RenameColumn(table: "dbo.Encounters", name: "Id", newName: "EncounterId");

            // EpisodeOfCare
            RenameColumn(table: "dbo.EpisodeOfCares", name: "Id", newName: "EpisodeOfCareId");
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "Id", newName: "EpisodeOfCareWorkflowId");

            // Comment
            RenameColumn(table: "dbo.Comments", name: "Id", newName: "CommentId");

            // GlobalThesaurusUser
            RenameColumn(table: "dbo.GlobalThesaurusRoles", name: "Id", newName: "GlobalThesaurusRoleId");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "Id", newName: "GlobalThesaurusUserRoleId");
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "Id", newName: "GlobalThesaurusUserId");

            // Organization
            RenameColumn(table: "dbo.Organizations", name: "Id", newName: "OrganizationId");
            RenameColumn(table: "dbo.OrganizationClinicalDomains", name: "Id", newName: "OrganizationClinicalDomainId");
            RenameColumn(table: "dbo.ClinicalDomains", name: "Id", newName: "ClinicalDomainId");
            RenameColumn(table: "dbo.OrganizationRelations", name: "Id", newName: "OrganizationRelationId");

            RenameColumn(table: "dbo.Identifiers", name: "Organization_Id", newName: "OrganizationId");
            RenameColumn(table: "dbo.Telecoms", name: "Organization_Id", newName: "OrganizationId");

            // Patient
            RenameColumn(table: "dbo.Communications", name: "Id", newName: "CommunicationId");
            RenameColumn(table: "dbo.Contacts", name: "Id", newName: "ContactId");
            RenameColumn(table: "dbo.MultipleBirths", name: "Id", newName: "MultipleBirthId");
            RenameColumn(table: "dbo.OutsideUsers", name: "Id", newName: "OutsideUserId");
            RenameColumn(table: "dbo.Patients", name: "Id", newName: "PatientId");
            RenameColumn(table: "dbo.PatientAddresses", name: "Id", newName: "PatientAddressId");
            RenameColumn(table: "dbo.PatientTelecoms", name: "Id", newName: "PatientTelecomId");
            RenameColumn(table: "dbo.Communications", name: "SmartOncologyPatient_Id", newName: "SmartOncologyPatientId");
            RenameColumn(table: "dbo.Communications", name: "Patient_Id", newName: "PatientId");

            RenameColumn(table: "dbo.SmartOncologyPatients", name: "ContactPerson_Id", newName: "ContactId");
            RenameColumn(table: "dbo.Identifiers", name: "SmartOncologyPatient_Id", newName: "SmartOncologyPatientId");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "MultipleB_Id", newName: "MultipleBirthId");
            RenameColumn(table: "dbo.Telecoms", name: "Contact_Id", newName: "ContactId");
            RenameColumn(table: "dbo.Patients", name: "ContactPerson_Id", newName: "ContactId");
            RenameColumn(table: "dbo.Identifiers", name: "Patient_Id", newName: "PatientId");
            RenameColumn(table: "dbo.Patients", name: "MultipleB_Id", newName: "MultipleBirthId");

            RenameColumn(table: "dbo.SmartOncologyPatients", name: "Id", newName: "SmartOncologyPatientId");
            RenameColumn(table: "dbo.EpisodeOfCares", name: "SmartOncologyPatient_Id", newName: "SmartOncologyPatientId");

            // ThesaurusEntry
            RenameColumn(table: "dbo.AdministrativeDatas", name: "Id", newName: "AdministrativeDataId");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "Id", newName: "ThesaurusEntryId");
            RenameColumn(table: "dbo.Versions", name: "Id", newName: "VersionId");
            RenameColumn(table: "dbo.O4CodeableConcept", name: "Id", newName: "O4CodeableConceptId");
            RenameColumn(table: "dbo.SimilarTerms", name: "Id", newName: "SimilarTermId");
            RenameColumn(table: "dbo.ThesaurusMerges", name: "Id", newName: "ThesaurusMergeId");
            RenameColumn(table: "dbo.O4CodeableConcept", name: "ThesaurusEntry_Id", newName: "ThesaurusEntryId");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "AdministrativeData_Id", newName: "AdministrativeDataId");

        }

        public override void Down()
        {
            // User
            RenameColumn(table: "dbo.Users", name: "UserId", newName: "Id");

            RenameColumn(table: "dbo.UserAcademicPositions", name: "UserAcademicPositionId", newName: "Id");
            RenameColumn(table: "dbo.AcademicPositionTypes", name: "AcademicPositionTypeId", newName: "Id");
            RenameColumn(table: "dbo.UserClinicalTrials", name: "UserClinicalTrialId", newName: "Id");
            RenameColumn(table: "dbo.UserOrganizations", name: "UserOrganizationId", newName: "Id");
            RenameColumn(table: "dbo.UserConfigs", name: "UserConfigId", newName: "Id");
            RenameColumn(table: "dbo.UserRoles", name: "UserRoleId", newName: "Id");

            // AccessManagement
            RenameColumn(table: "dbo.Permissions", name: "PermissionId", newName: "Id");
            RenameColumn(table: "dbo.PermissionModules", name: "PermissionModuleId", newName: "Id");
            RenameColumn(table: "dbo.Modules", name: "ModuleId", newName: "Id");
            RenameColumn(table: "dbo.PermissionRoles", name: "PermissionRoleId", newName: "Id");
            RenameColumn(table: "dbo.Roles", name: "RoleId", newName: "Id");

            // ChemotherapySchema
            RenameColumn(table: "dbo.RouteOfAdministrations", name: "RouteOfAdministrationId", newName: "Id");
            RenameColumn(table: "dbo.MedicationDoseTypes", name: "MedicationDoseTypeId", newName: "Id");
            RenameColumn(table: "dbo.MedicationDoseTimes", name: "MedicationDoseTimeId", newName: "Id");
            RenameColumn(table: "dbo.MedicationDoses", name: "MedicationDoseId", newName: "Id");
            RenameColumn(table: "dbo.Units", name: "UnitId", newName: "Id");
            RenameColumn(table: "dbo.Medications", name: "MedicationId", newName: "Id");
            RenameColumn(table: "dbo.LiteratureReferences", name: "LiteratureReferenceId", newName: "Id");
            RenameColumn(table: "dbo.Indications", name: "IndicationId", newName: "Id");
            RenameColumn(table: "dbo.ChemotherapySchemas", name: "ChemotherapySchemaId", newName: "Id");
            RenameColumn(table: "dbo.BodySurfaceCalculationFormulas", name: "BodySurfaceCalculationFormulaId", newName: "Id");

            //ChemotherapySchemaInstance
            RenameColumn(table: "dbo.MedicationDoseTimeInstances", name: "MedicationDoseTimeInstanceId", newName: "Id");
            RenameColumn(table: "dbo.MedicationDoseInstances", name: "MedicationDoseInstanceId", newName: "Id");
            RenameColumn(table: "dbo.MedicationInstances", name: "MedicationInstanceId", newName: "Id");
            RenameColumn(table: "dbo.MedicationReplacements", name: "MedicationReplacementId", newName: "Id");
            RenameColumn(table: "dbo.ChemotherapySchemaInstanceVersions", name: "ChemotherapySchemaInstanceVersionId", newName: "Id");
            RenameColumn(table: "dbo.ChemotherapySchemaInstances", name: "ChemotherapySchemaInstanceId", newName: "Id");

            // CodeSystem
            RenameColumn(table: "dbo.CodeSystems", name: "CodeSystemId", newName: "Id");

            // Common
            RenameColumn(table: "dbo.Telecoms", name: "TelecomId", newName: "Id");
            RenameColumn(table: "dbo.Identifiers", name: "IdentifierId", newName: "Id");
            RenameColumn(table: "dbo.CustomEnums", name: "CustomEnumId", newName: "Id");
            RenameColumn(table: "dbo.Addresses", name: "AddressId", newName: "Id");

            RenameColumn(table: "dbo.OutsideUsers", name: "AddressId", newName: "Address_Id");
            RenameColumn(table: "dbo.Contacts", name: "AddressId", newName: "Address_Id");

            // Encounter
            RenameColumn(table: "dbo.Encounters", name: "EncounterId", newName: "Id");

            // EpisodeOfCare
            RenameColumn(table: "dbo.EpisodeOfCareWorkflows", name: "EpisodeOfCareWorkflowId", newName: "Id");
            RenameColumn(table: "dbo.EpisodeOfCares", name: "EpisodeOfCareId", newName: "Id");

            // Comment
            RenameColumn(table: "dbo.Comments", name: "CommentId", newName: "Id");

            // GlobalThesaurusUser
            RenameColumn(table: "dbo.GlobalThesaurusUsers", name: "GlobalThesaurusUserId", newName: "Id");
            RenameColumn(table: "dbo.GlobalThesaurusUserRoles", name: "GlobalThesaurusUserRoleId", newName: "Id");
            RenameColumn(table: "dbo.GlobalThesaurusRoles", name: "GlobalThesaurusRoleId", newName: "Id");

            // Organization
            RenameColumn(table: "dbo.OrganizationRelations", name: "OrganizationRelationId", newName: "Id");
            RenameColumn(table: "dbo.ClinicalDomains", name: "ClinicalDomainId", newName: "Id");
            RenameColumn(table: "dbo.OrganizationClinicalDomains", name: "OrganizationClinicalDomainId", newName: "Id");
            RenameColumn(table: "dbo.Organizations", name: "OrganizationId", newName: "Id");

            RenameColumn(table: "dbo.Telecoms", name: "OrganizationId", newName: "Organization_Id");
            RenameColumn(table: "dbo.Identifiers", name: "OrganizationId", newName: "Organization_Id");

            //Patient
            RenameColumn(table: "dbo.Communications", name: "PatientId", newName: "Patient_Id");
            RenameColumn(table: "dbo.Communications", name: "SmartOncologyPatientId", newName: "SmartOncologyPatient_Id");
            RenameColumn(table: "dbo.PatientTelecoms", name: "PatientTelecomId", newName: "Id");
            RenameColumn(table: "dbo.PatientAddresses", name: "PatientAddressId", newName: "Id");
            RenameColumn(table: "dbo.Patients", name: "PatientId", newName: "Id");
            RenameColumn(table: "dbo.OutsideUsers", name: "OutsideUserId", newName: "Id");
            RenameColumn(table: "dbo.MultipleBirths", name: "MultipleBirthId", newName: "Id");
            RenameColumn(table: "dbo.Contacts", name: "ContactId", newName: "Id");
            RenameColumn(table: "dbo.Communications", name: "CommunicationId", newName: "Id");

            RenameColumn(table: "dbo.Patients", name: "MultipleBirthId", newName: "MultipleB_Id");
            RenameColumn(table: "dbo.Identifiers", name: "PatientId", newName: "Patient_Id");
            RenameColumn(table: "dbo.Patients", name: "ContactId", newName: "ContactPerson_Id");
            RenameColumn(table: "dbo.Telecoms", name: "ContactId", newName: "Contact_Id");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "MultipleBirthId", newName: "MultipleB_Id");
            RenameColumn(table: "dbo.Identifiers", name: "SmartOncologyPatientId", newName: "SmartOncologyPatient_Id");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "ContactId", newName: "ContactPerson_Id");

            RenameColumn(table: "dbo.EpisodeOfCares", name: "SmartOncologyPatientId", newName: "SmartOncologyPatient_Id");
            RenameColumn(table: "dbo.SmartOncologyPatients", name: "SmartOncologyPatientId", newName: "Id");

            // ThesaurusEntry
            RenameColumn(table: "dbo.AdministrativeDatas", name: "AdministrativeDataId", newName: "Id");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "AdministrativeDataId", newName: "AdministrativeData_Id");
            RenameColumn(table: "dbo.O4CodeableConcept", name: "ThesaurusEntryId", newName: "ThesaurusEntry_Id");
            RenameColumn(table: "dbo.ThesaurusMerges", name: "ThesaurusMergeId", newName: "Id");
            RenameColumn(table: "dbo.SimilarTerms", name: "SimilarTermId", newName: "Id");
            RenameColumn(table: "dbo.O4CodeableConcept", name: "O4CodeableConceptId", newName: "Id");
            RenameColumn(table: "dbo.Versions", name: "VersionId", newName: "Id");
            RenameColumn(table: "dbo.ThesaurusEntries", name: "ThesaurusEntryId", newName: "Id");

        }
    }
}
