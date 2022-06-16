using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Serilog;
using sReportsV2.BusinessLayer.Implementations;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Configs;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.MapperProfiles;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.UMLS.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Initializer.PredefinedTypes;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Singleton;
using sReportsV2.Common.Constants;

namespace sReportsV2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(x =>
            {
                x.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            });
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperWebConfiguration.Configure();
            SerilogConfiguration.ConfigureWritingToFile();
            MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"];
            string sqlConnectionString = ConfigurationManager.AppSettings["Sql"];
            RegisterAutofac();           
            PopulateUsers();
            PopulateEnums();
            PopulateInitialData();
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
           
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Language"];

            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["DefaultLanguage"]);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["DefaultLanguage"]);
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            
        }

        private void PopulateUsers()
        {
            var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
            if (userDAL.CountAll() == 0)
            {
                var organizationDAL = DependencyResolver.Current.GetService<IOrganizationDAL>();
                Organization organization = new Organization()
                {
                    Name = "weMedoo AG",
                    Address = new Domain.Sql.Entities.Common.Address()
                    {
                        City = "Zurich",
                        Country = "Switzerland",
                        PostalCode = "6312",
                        Street = "Sumpfstrasse 24",
                        State = "Switzerland",
                        StreetNumber = 24
                    },
                    EntryDatetime = DateTime.Now
                };

                organizationDAL.InsertOrUpdate(organization);

                User user = new User()
                {
                    Username = "smladen",
                    Password = "test",
                    FirstName = "Mladen",
                    LastName = "Stanojevic",
                    Email = "test@gmail.com",
                    EntryDatetime = DateTime.Now,
                    DayOfBirth = DateTime.Now,
                    UserConfig = new UserConfig()
                    {
                        ActiveOrganizationId = organization.Id,
                        ActiveLanguage = "en"
                    },
                };

                userDAL.InsertOrUpdate(user);
                UserOrganization userOrganization = new UserOrganization()
                {
                    UserId = user.Id,
                    OrganizationId = organization.Id,
                    State = Common.Enums.UserState.Active
                };

                user.Organizations.Add(userOrganization);

                userDAL.InsertOrUpdate(user);
            }
        }

        private void PopulateEnums()
        {            
            var clinicalDomainDAL = DependencyResolver.Current.GetService<IClinicalDomainDAL>();
            if(clinicalDomainDAL.Count() == 0)
            {
                var values = Enum.GetValues(typeof(DocumentClinicalDomain));
                foreach (DocumentClinicalDomain domain in values)
                {
                    ClinicalDomain clinicalDomain = new ClinicalDomain()
                    {
                        Id = (int)domain,
                        Name = domain.ToString()
                    };
                    clinicalDomainDAL.Insert(clinicalDomain);
                }
            }
        }


        private void PopulateInitialData()
        {
            if(ConfigurationManager.AppSettings["Instance"] == InstanceNames.ThesaurusGlobal)
            {
                SqlImporter thesaurusImporter = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
                DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
                DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
                DependencyResolver.Current.GetService<ICodeSystemDAL>());
                thesaurusImporter.Import(BlobStorageHelper.GetUrl("UMLS"));
            }

            PredefineTypesImporter importer = new PredefineTypesImporter(DependencyResolver.Current.GetService<ICustomEnumDAL>(), DependencyResolver.Current.GetService<IThesaurusDAL>());
            importer.Import();


            SqlImporter sqlImporter = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
                DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
                DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
                DependencyResolver.Current.GetService<ICodeSystemDAL>());
            sqlImporter.ImportCodingSystems(BlobStorageHelper.GetUrl("UMLS"));


            SingletonDataContainer.Instance.RefreshSingleton();
        }

        private void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<SReportsContext>().InstancePerLifetimeScope();
            builder.RegisterType<UserDAL>().As<IUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OrganizationDAL>().As<IOrganizationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<GlobalUserDAL>().As<IGlobalUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusDAL>().As<IThesaurusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ThesaurusTranslationDAL>().As<IThesaurusTranslationDAL>().InstancePerLifetimeScope();
            builder.RegisterType<O4CodeableConceptDAL>().As<IO4CodeableConceptDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CodeSystemDAL>().As<ICodeSystemDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormDAL>().As<IFormDAL>().InstancePerLifetimeScope();
            builder.RegisterType<FormInstanceDAL>().As<IFormInstanceDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CommentDAL>().As<ICommentDAL>().InstancePerLifetimeScope();
            builder.RegisterType<CustomEnumDAL>().As<ICustomEnumDAL>().InstancePerLifetimeScope();
            builder.RegisterType<OutsideUserDAL>().As<IOutsideUserDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ConsensusDAL>().As<IConsensusDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EncounterDAL>().As<IEncounterDAL>().InstancePerLifetimeScope();
            builder.RegisterType<EpisodeOfCareDAL>().As<IEpisodeOfCareDAL>().InstancePerLifetimeScope();
            builder.RegisterType<PatientDAL>().As<IPatientDAL>().InstancePerLifetimeScope();

            builder.RegisterType<UserBLL>().As<IUserBLL>();

            builder.RegisterType<OrganizationBLL>().As<IOrganizationBLL>();
            builder.RegisterType<ThesaurusEntryBLL>().As<IThesaurusEntryBLL>();
            builder.RegisterType<CustomEnumBLL>().As<ICustomEnumBLL>();
            builder.RegisterType<FormInstanceBLL>().As<IFormInstanceBLL>();
            builder.RegisterType<FormBLL>().As<IFormBLL>();
            builder.RegisterType<CommentBLL>().As<ICommentBLL>();
            builder.RegisterType<PatientBLL>().As<IPatientBLL>();
            builder.RegisterType<PdfBLL>().As<IPdfBLL>();

            builder.RegisterType<OrganizationRelationDAL>().As<IOrganizationRelationDAL>();
            builder.RegisterType<AddressDAL>().As<IAddressDAL>();
            builder.RegisterType<ClinicalDomainDAL>().As<IClinicalDomainDAL>();
            builder.RegisterModule(new AutofacWebTypesModule());

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
