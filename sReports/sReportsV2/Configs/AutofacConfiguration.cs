using Autofac;
using Autofac.Integration.Mvc;
using sReportsV2.BusinessLayer.Implementations;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System.Web.Mvc;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Common.Helpers.EmailSender.Implementations;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using System.Configuration;
using sReportsV2.Common.Constants;

namespace sReportsV2.Configs
{
    public static class AutofacConfiguration
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<SReportsContext>().InstancePerLifetimeScope();
            RegisterCustomComponents(builder);
            RegisterBLLs(builder);
            RegisterDALs(builder);

            builder.RegisterModule(new AutofacWebTypesModule());
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterCustomComponents(ContainerBuilder builder)
        {

            if (ConfigurationManager.AppSettings["EmailSender"] == EmailSenderNames.SmtpEmailSender)
            {
                builder.RegisterType<SmtpEmailSender>().As<IEmailSender>();
            }
            else
            {
                builder.RegisterType<SendGridEmailSender>().As<IEmailSender>();
            }
        }

        private static void RegisterBLLs(ContainerBuilder builder)
        {
            builder.RegisterType<UserBLL>().As<IUserBLL>();
            builder.RegisterType<RoleBLL>().As<IRoleBLL>();
            builder.RegisterType<OrganizationBLL>().As<IOrganizationBLL>();
            builder.RegisterType<ThesaurusEntryBLL>().As<IThesaurusEntryBLL>();
            builder.RegisterType<CustomEnumBLL>().As<ICustomEnumBLL>();
            builder.RegisterType<FormInstanceBLL>().As<IFormInstanceBLL>();
            builder.RegisterType<FormBLL>().As<IFormBLL>();
            builder.RegisterType<CommentBLL>().As<ICommentBLL>();
            builder.RegisterType<PatientBLL>().As<IPatientBLL>();
            builder.RegisterType<PdfBLL>().As<IPdfBLL>();
            builder.RegisterType<ConsensusBLL>().As<IConsensusBLL>();
            builder.RegisterType<FormDistributionBLL>().As<IFormDistributionBLL>();
            builder.RegisterType<EncounterBLL>().As<IEncounterBLL>();
            builder.RegisterType<EpisodeOfCareBLL>().As<IEpisodeOfCareBLL>();
            builder.RegisterType<DigitalGuidelineBLL>().As<IDigitalGuidelineBLL>();
            builder.RegisterType<DigitalGuidelineInstanceBLL>().As<IDigitalGuidelineInstanceBLL>();
            builder.RegisterType<SmartOncologyPatientBLL>().As<ISmartOncologyPatientBLL>();
            builder.RegisterType<GlobalUserBLL>().As<IGlobalUserBLL>();
            builder.RegisterType<ChemotherapySchemaBLL>().As<IChemotherapySchemaBLL>();
            builder.RegisterType<ChemotherapySchemaInstanceBLL>().As<IChemotherapySchemaInstanceBLL>();

            _ = bool.TryParse(ConfigurationManager.AppSettings["UseFileStorage"], out bool useFileStorage);
            if (useFileStorage)
            {
                builder.RegisterType<FileStorageBLL>().As<IBlobStorageBLL>();
            }
            else
            {
                builder.RegisterType<CloudStorageBLL>().As<IBlobStorageBLL>();
            }
        }
        private static void RegisterDALs(ContainerBuilder builder)
        {
            builder.RegisterType<UserDAL>().As<IUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationDAL>().As<IOrganizationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalThesaurusUserDAL>().As<IGlobalThesaurusUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusDAL>().As<IThesaurusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusTranslationDAL>().As<IThesaurusTranslationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<AdministrativeDataDAL>().As<IAdministrativeDataDAL>().InstancePerLifetimeScope();
            builder.RegisterType<O4CodeableConceptDAL>().As<IO4CodeableConceptDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeSystemDAL>().As<ICodeSystemDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormDAL>().As<IFormDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormInstanceDAL>().As<IFormInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CommentDAL>().As<ICommentDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CustomEnumDAL>().As<ICustomEnumDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OutsideUserDAL>().As<IOutsideUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ConsensusDAL>().As<IConsensusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ConsensusInstanceDAL>().As<IConsensusInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EncounterDAL>().As<IEncounterDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EpisodeOfCareDAL>().As<IEpisodeOfCareDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PatientDAL>().As<IPatientDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationRelationDAL>().As<IOrganizationRelationDAL>();
            builder.RegisterType<AddressDAL>().As<IAddressDAL>();
            builder.RegisterType<ClinicalDomainDAL>().As<IClinicalDomainDAL>();
            builder.RegisterType<RoleDAL>().As<IRoleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionRoleDAL>().As<IPermissionRoleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ModuleDAL>().As<IModuleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionDAL>().As<IPermissionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionModuleDAL>().As<IPermissionModuleDAL>().InstancePerLifetimeScope();
            builder.RegisterType<AcademicPositionDAL>().As<IAcademicPositionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusMergeDAL>().As<IThesaurusMergeDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormDistributionDAL>().As<IFormDistributionDAL>().InstancePerLifetimeScope();
            builder.RegisterType<DigitalGuidelineDAL>().As<IDigitalGuidelineDAL>().InstancePerLifetimeScope();
            builder.RegisterType<DigitalGuidelineInstanceDAL>().As<IDigitalGuidelineInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<SmartOncologyPatientDAL>().As<ISmartOncologyPatientDAL>();
            builder.RegisterType<GlobalThesaurusUserDAL>().As<IGlobalThesaurusUserDAL>();
            builder.RegisterType<GlobalThesaurusRoleDAL>().As<IGlobalThesaurusRoleDAL>();
            builder.RegisterType<ChemotherapySchemaDAL>().As<IChemotherapySchemaDAL>();
            builder.RegisterType<ChemotherapySchemaInstanceDAL>().As<IChemotherapySchemaInstanceDAL>();
            builder.RegisterType<ChemotherapySchemaInstanceHistoryDAL>().As<IChemotherapySchemaInstanceHistoryDAL>();
            builder.RegisterType<LiteratureReferenceDAL>().As<ILiteratureReferenceDAL>();
            builder.RegisterType<MedicationDAL>().As<IMedicationDAL>();
            builder.RegisterType<MedicationInstanceDAL>().As<IMedicationInstanceDAL>();
            builder.RegisterType<MedicationReplacementDAL>().As<IMedicationReplacementDAL>();
            builder.RegisterType<BodySurfaceCalculationFormulaDAL>().As<IBodySurfaceCalculationFormulaDAL>();
            builder.RegisterType<RouteOfAdministrationDAL>().As<IRouteOfAdministrationDAL>();
            builder.RegisterType<MedicationDoseTypeDAL>().As<IMedicationDoseTypeDAL>();
            builder.RegisterType<MedicationDoseDAL>().As<IMedicationDoseDAL>();
            builder.RegisterType<MedicationDoseInstanceDAL>().As<IMedicationDoseInstanceDAL>();
            builder.RegisterType<UnitDAL>().As<IUnitDAL>();
            builder.RegisterType<CustomFieldFilterDAL>().As<ICustomFieldFilterDAL>().InstancePerLifetimeScope();
        }
    }
}