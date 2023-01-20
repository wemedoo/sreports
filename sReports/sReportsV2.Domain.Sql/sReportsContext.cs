using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Consensus;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.SmartOncologyPatient;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using static System.Data.Entity.Migrations.Model.UpdateDatabaseOperation;

namespace sReportsV2.DAL.Sql.Sql
{
    public class SReportsContext : DbContext
    {
        public SReportsContext() : base(ConfigurationManager.AppSettings["Sql"]) 
        {
            //Database.Log = s => Debug.WriteLine(s);
            //discuss this how long should be set
            this.Database.CommandTimeout = 180;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SReportsContext, Domain.Sql.Migrations.Configuration>(useSuppliedContext: true));

        }

        public DbSet<ThesaurusEntry> Thesauruses { get; set; }
        public DbSet<ThesaurusEntryTranslation> ThesaurusEntryTranslations { get; set; }
        public DbSet<SimilarTerm> SimilarTerms { get; set; }
        public DbSet<Domain.Sql.Entities.ThesaurusEntry.Version> Versions { get; set; }
        public DbSet<AdministrativeData> AdministrativeDatas { get; set; }
        public DbSet<O4CodeableConcept> O4CodeableConcept { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<OrganizationRelation> OrganizationRelation { get; set; }
        public DbSet<OrganizationClinicalDomain> OrganizationClinicalDomain { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<UserClinicalTrial> UserClinicalTrial { get; set; }
        public DbSet<CustomEnum> CustomEnums { get; set; }
        public DbSet<Identifier> Identifiers { get; set; }
        public DbSet<GlobalThesaurusUser> GlobalUser { get; set; }
        public DbSet<CodeSystem> CodeSystems { get; set; }
        public DbSet<ClinicalDomain> ClinicalDomain { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<OutsideUser> OutsideUsers { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<EpisodeOfCare> EpisodeOfCares { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<SmartOncologyPatient> SmartOncologyPatients { get; set; }
        public DbSet<Telecom> Telecoms { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<AcademicPositionType> AcademicPositionTypes { get; set; }
        public DbSet<ThesaurusMerge> ThesaurusMerge { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionModule> PermissionModules { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<GlobalThesaurusUserRole> GlobalThesaurusUserRoles { get; set; }
        public DbSet<GlobalThesaurusRole> GlobalThesaurusRoles { get; set; }
        public DbSet<ChemotherapySchema> ChemotherapySchemas { get; set; }
        public DbSet<ChemotherapySchemaInstance> ChemotherapySchemaInstances { get; set; }
        public DbSet<ChemotherapySchemaInstanceVersion> ChemotherapySchemaInstanceVersions { get; set; }
        public DbSet<LiteratureReference> LiteratureReferences { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationDose> MedicationDoses { get; set; }
        public DbSet<MedicationInstance> MedicationInstances { get; set; }
        public DbSet<MedicationDoseInstance> MedicationDoseInstances { get; set; }
        public DbSet<BodySurfaceCalculationFormula> BodySurfaceCalculationFormulas { get; set; }
        public DbSet<RouteOfAdministration> RouteOfAdministrations { get; set; }
        public DbSet<MedicationDoseType> MedicationDoseTypes { get; set; }
        public DbSet<MedicationReplacement> MedicationReplacements { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserView> UserViews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationRelation>()
            .HasRequired(c => c.Child)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrganizationRelation>()
            .HasRequired(c => c.Parent)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChemotherapySchemaInstance>()
            .HasRequired(s => s.Creator)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
            .HasOptional(c => c.Address)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
            .HasOptional(c => c.CreatedBy)
            .WithMany()
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserAcademicPosition>()
            .HasOptional(c => c.CreatedBy)
            .WithMany()
            .WillCascadeOnDelete(false);
        }
    }
}
