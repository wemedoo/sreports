using Newtonsoft.Json;
using Serilog;
using sReportsV2.Configs;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.UMLS.Classes;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Initializer.PredefinedTypes;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using System.Collections.Generic;
using sReportsV2.Initializer.AccessManagment;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using System.Linq;
using ExcelImporter.Importers;
using ExcelImporter.Constants;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System.Web.SessionState;
using sReportsV2.Domain.DatabaseMigrationScripts;

namespace sReportsV2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const string defaultUsername = "nikola.cihoric";
        private const string defaultOrganization = "weMedoo AG";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(x =>
            {
                x.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            });
            MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"];

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperWebConfiguration.Configure();
            SerilogConfiguration.ConfigureWritingToFile();
            AutofacConfiguration.Configure();
            new MongoMigrator().SetToLatestVersion();

            if (ConfigurationManager.AppSettings["Instance"] == InstanceNames.ThesaurusGlobal)
            {
                InitializeGlobalThesarus();
            }
            else
            {
                Initialize();
            } 
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

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["DefaultLanguage"]);
            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["DefaultLanguage"]);
            }
        }

        protected void Application_EndRequest()
        {
            // Any AJAX request that ends in a redirect should get mapped to an unauthorized request
            // since it should only happen when the request is not authorized and gets automatically
            // redirected to the login page.
            var context = new HttpContextWrapper(Context);
            if (context.Response.StatusCode == 302 && context.Request.IsAjaxRequest())
            {
                context.Response.Clear();
                Context.Response.StatusCode = 401;
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            
        }
        /*protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }*/

        #region default_initialisation
        private void Initialize()
        {
            PopulateRoles();
            PopulateUsers();
            PopulateEnums();
            PopulateInitialData();
            PopulateResourcesWithOrganizationIfNotSet();
        }

        private void PopulateRoles()
        {
            RoleInitializer roleInitializer = new RoleInitializer();
            roleInitializer.SetInitialData();
        }

        private void PopulateUsers()
        {
            var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
            userDAL.UpdateUsersCountForAllOrganization();

            var customEnumDAL = DependencyResolver.Current.GetService<ICustomEnumDAL>();
            int? countryId = customEnumDAL.GetIdByTypeAndPreferredTerm(ResourceTypes.CompanyCountry, Common.Enums.CustomEnumType.Country);

            if (userDAL.CountAll() == 0)
            {
                var organizationDAL = DependencyResolver.Current.GetService<IOrganizationDAL>();
                Organization organization = new Organization()
                {
                    Name = "weMedoo AG",
                    Address = new Domain.Sql.Entities.Common.Address()
                    {
                        City = "Zurich",
                        CountryId = countryId,
                        PostalCode = "6312",
                        Street = "Sumpfstrasse 24",
                        State = ResourceTypes.CompanyCountry,
                        StreetNumber = 24
                    },
                };

                organizationDAL.InsertOrUpdate(organization);
                string salt = PasswordHelper.CreateSalt(10);
                User user = new User()
                {
                    Username = defaultUsername,
                    Salt = salt,
                    Password = PasswordHelper.Hash("test", salt),
                    FirstName = "Nikola",
                    LastName = "Cihoric",
                    Email = "nikola.cihoric@insel.ch",
                    DayOfBirth = DateTime.Now,
                    UserConfig = new UserConfig()
                    {
                        ActiveOrganizationId = organization.OrganizationId,
                        ActiveLanguage = "en"
                    },
                };

                userDAL.InsertOrUpdate(user);
                UserOrganization userOrganization = new UserOrganization()
                {
                    UserId = user.UserId,
                    OrganizationId = organization.OrganizationId,
                    State = Common.Enums.UserState.Active
                };

                user.Organizations.Add(userOrganization);

                userDAL.InsertOrUpdate(user);
            }
            AssignRolesIfNotSet(userDAL);
        }

        private void AssignRolesIfNotSet(IUserDAL userDAL)
        {
            var roleDAL = DependencyResolver.Current.GetService<IRoleDAL>();
            var superAdministratorRole = roleDAL.GetByName(PredifinedRole.SuperAdministrator.ToString());
            var user = userDAL.GetByUsername(defaultUsername);
            if (user != null && superAdministratorRole != null && user.Roles.Count == 0)
            {
                user.UpdateRoles(new List<int>() { superAdministratorRole.RoleId });
                userDAL.InsertOrUpdate(user);
            }
        }

        private void PopulateEnums()
        {            
            var clinicalDomainDAL = DependencyResolver.Current.GetService<IClinicalDomainDAL>();
  
            var values = Enum.GetValues(typeof(DocumentClinicalDomain));
            foreach (DocumentClinicalDomain domain in values)
            {
                clinicalDomainDAL.Insert(domain);
            }

            var acDAL = DependencyResolver.Current.GetService<IAcademicPositionDAL>();
            if (acDAL.Count() == 0)
            {
                var aPositions = Enum.GetValues(typeof(AcademicPosition));
                foreach (AcademicPosition ac in aPositions)
                {
                    AcademicPositionType role = new AcademicPositionType()
                    {
                        AcademicPositionTypeId = (int)ac,
                        Name = ac.ToString()
                    };
                    acDAL.Insert(role);
                }
            }
        }

        private void PopulateInitialData()
        {
            InsertPredefinedTypes();
            InsertCodingSystems();
            SetMissingThesaurusIdsListForForms();
            PopulateChemotherapySchemaInitialData();

            SingletonDataContainer.Instance.RefreshSingleton();
        }

        private void InsertPredefinedTypes()
        {
            var customEnumDAL = DependencyResolver.Current.GetService<ICustomEnumDAL>();
            var thesaurusDAL = DependencyResolver.Current.GetService<IThesaurusDAL>();
            PredefinedTypesImporter importer = new PredefinedTypesImporter(customEnumDAL, thesaurusDAL);
            importer.Import();

            InsertPredefinedTypesFromExcel(customEnumDAL, thesaurusDAL);
        }

        private void InsertPredefinedTypesFromExcel(ICustomEnumDAL customEnumDAL, IThesaurusDAL thesaurusDAL)
        {
            var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
            User user = userDAL.GetByUsername(defaultUsername);
            var organizationDAL = DependencyResolver.Current.GetService<IOrganizationDAL>();
            Organization organization = organizationDAL.GetByName(defaultOrganization);
            string fileAndSheetName = "CountryCodes";

            if(user != null && organization != null)
            {
                CountryCodeImporter predefinedTypeExcelImporter = new CountryCodeImporter(fileAndSheetName, fileAndSheetName, thesaurusDAL, customEnumDAL, DependencyResolver.Current.GetService<ICodeSystemDAL>(), DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(), DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(), DependencyResolver.Current.GetService<IAdministrativeDataDAL>(), user.UserId, organization.OrganizationId);
                predefinedTypeExcelImporter.ImportDataFromExcelToDatabase();
            }
        }

        private void InsertCodingSystems()
        {
            SqlImporter importer = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
                DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
                DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
                DependencyResolver.Current.GetService<ICodeSystemDAL>(), DependencyResolver.Current.GetService<IAdministrativeDataDAL>());
            importer.ImportCodingSystems();
        }

        private void SetMissingThesaurusIdsListForForms()
        {
            IFormDAL formDAL = new FormDAL();
            foreach (Domain.Entities.Form.Form form in formDAL.GetAllWithEmptyThesaurusIdsList())
            {
                formDAL.InsertOrUpdate(form, new Common.Entities.User.UserData { Id = form.UserId }, false);
            }
        }

        #region smart_oncology_initialisation

        private void PopulateChemotherapySchemaInitialData()
        {
            AddUnits();
            ImportChemotherapySchemaDataFromExcel();
            ImportChemotherapySchemasFromExcel();
            UpdateDosingTime();
        }

        private void AddUnits()
        {
            var unitDAL = DependencyResolver.Current.GetService<IUnitDAL>();
            if(unitDAL.GetAllCount() == 0)
            {
                List<Unit> units = new List<Unit> {
                    {
                        new Unit { Name = "mg", Description = "A unit of measurement of mass in the metric system equal to a thousandth of a gram"}
                    },
                    {
                        new Unit { Name = "g", Description = "A unit of measurement of mass in the metric system"}
                    },
                    {
                        new Unit { Name = "IU", Description = "An abbreviations which stands for International Units"}
                    },
                    {
                        new Unit { Name = "ml", Description = "A metric unit used to measure capacity that's equal to one-thousandth of a liter"}
                    },
                    {
                        new Unit { Name = "mg/m2", Description = "A unit for Body Surface Area Based Dosing"}
                    },
                    {
                        new Unit { Name = "AUC", Description = "An abbreviations for Area Under the curve"}
                    }
                };
                unitDAL.InsertMany(units);
            }
        }

        private void ImportChemotherapySchemaDataFromExcel()
        {
            var bodySurfaceCalculationFormulaDAL = DependencyResolver.Current.GetService<IBodySurfaceCalculationFormulaDAL>();
            BodySurfaceCalculationFormulaImporter bodySurfaceCalculationFormulaImporter = new BodySurfaceCalculationFormulaImporter(bodySurfaceCalculationFormulaDAL, ChemotherapySchemaConstants.ChemOncAdditionalDataFile, ChemotherapySchemaConstants.BodySurfaceCalculationFormulaSheet);
            bodySurfaceCalculationFormulaImporter.ImportDataFromExcelToDatabase();

            var routeOfAdministrationDAL = DependencyResolver.Current.GetService<IRouteOfAdministrationDAL>();
            RouteOfAdministrationImporter routeOfAdministrationImporter = new RouteOfAdministrationImporter(routeOfAdministrationDAL, ChemotherapySchemaConstants.ChemOncAdditionalDataFile, ChemotherapySchemaConstants.RouteOfAdministrationSheet);
            routeOfAdministrationImporter.ImportDataFromExcelToDatabase();

            var medicationDoseTypeDAL = DependencyResolver.Current.GetService<IMedicationDoseTypeDAL>();
            MedicationDoseTypeImporter medicationDoseTypeImporter = new MedicationDoseTypeImporter(medicationDoseTypeDAL, ChemotherapySchemaConstants.ChemOncDrugDosingTimeFile, ChemotherapySchemaConstants.DrugDosingTimeSheet);
            medicationDoseTypeImporter.ImportDataFromExcelToDatabase();
        }

        private void ImportChemotherapySchemasFromExcel()
        {
            var userDAL = DependencyResolver.Current.GetService<IUserDAL>();
            User user = userDAL.GetByUsername(defaultUsername);
            if (user != null)
            {
                SchemaImporterV2 schemaImporterV2 = new SchemaImporterV2("Chemotherapy Compendium Import - 26.11.2021", "Basic Data", DependencyResolver.Current.GetService<IChemotherapySchemaDAL>(), DependencyResolver.Current.GetService<IRouteOfAdministrationDAL>(), 
                  DependencyResolver.Current.GetService<IUnitDAL>(),
                user.UserId);
                schemaImporterV2.ImportDataFromExcelToDatabase();
            }
        }

        private void UpdateDosingTime()
        {
            var medicationDoseTypeDAL = DependencyResolver.Current.GetService<IMedicationDoseTypeDAL>();
            var dosingTime = medicationDoseTypeDAL.GetByName("Three times daily, Start at 08:00");
            if (dosingTime != null)
            {
                if(dosingTime.IntervalsList.Any(t => t.Equals("9:00")))
                {
                    // Fix first dose time
                    dosingTime.IntervalsList = new List<string>() { { "8:00" }, { "13:00" }, { "20:00" } };
                    medicationDoseTypeDAL.InsertOrUpdate(dosingTime);
                }
            }
        }

        #endregion

        private void PopulateResourcesWithOrganizationIfNotSet()
        {
            var organizationDAL = DependencyResolver.Current.GetService<IOrganizationDAL>();
            Organization organization = organizationDAL.GetByName(defaultOrganization);

            if (organization != null)
            {
                var patientDAL = DependencyResolver.Current.GetService<IPatientDAL>();

                PatientFilter patientFilter = new PatientFilter { OrganizationId = 0, Page = 1 };
                int patientsCount = patientDAL.GetAllEntriesCount(patientFilter);
                patientFilter.PageSize = patientsCount;
                List<Patient> patientsUnassigned = patientDAL.GetAll(patientFilter);
                patientsUnassigned.ForEach(x => {
                    x.OrganizationId = organization.OrganizationId;
                    patientDAL.InsertOrUpdate(x);
                });
            }
        }

        #endregion

        #region global_thesaurus_initialisation
        private void InitializeGlobalThesarus()
        {
            PopulateGlobalThesaurusRoles();
            SetGlobalThesaurusUsers();
            PopulateGlobalThesaurusInitialData();
        }

        private void PopulateGlobalThesaurusRoles()
        {
            IGlobalThesaurusRoleDAL globalThesaurusRoleDAL = DependencyResolver.Current.GetService<IGlobalThesaurusRoleDAL>();

            if (globalThesaurusRoleDAL.Count() == 0)
            {

                GlobalThesaurusRole roleSuperAdministrator = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.SuperAdministrator.ToString(),
                    Description = "A super administrator is a person who has full access to user and any other modules. Super administrator can manage user's roles."
                };
                GlobalThesaurusRole roleViewer = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Viewer.ToString(),
                    Description = "A viewer can access to the smart oncology content and view it."
                };
                GlobalThesaurusRole roleEditor = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Editor.ToString(),
                    Description = "A editor can edit terminology terms."
                };
                GlobalThesaurusRole roleCurator = new GlobalThesaurusRole
                {
                    Name = PredifinedGlobalUserRole.Curator.ToString(),
                    Description = "A curator can edit and cure terms."
                };

                globalThesaurusRoleDAL.InsertOrUpdate(roleSuperAdministrator);
                globalThesaurusRoleDAL.InsertOrUpdate(roleViewer);
                globalThesaurusRoleDAL.InsertOrUpdate(roleEditor);
                globalThesaurusRoleDAL.InsertOrUpdate(roleCurator);
            }
        }

        private void SetGlobalThesaurusUsers()
        {
            var userRoleDAL = DependencyResolver.Current.GetService<IGlobalThesaurusRoleDAL>();
            var roles = userRoleDAL.GetAll().ToList();
            var userDAL = DependencyResolver.Current.GetService<IGlobalThesaurusUserDAL>();
            SetGlobalThesaurusSuperAdministrators(userDAL, roles);
            SetGlobalThesaurusOtherUsers(userDAL, roles);
        }

        private void SetGlobalThesaurusSuperAdministrators(IGlobalThesaurusUserDAL userDAL, List<GlobalThesaurusRole> roles)
        {
            List<int> roleIds = roles.Select(x => x.GlobalThesaurusRoleId).ToList();
            foreach (var superAdministrator in GetGlobalThesaurusSuperAdministrators())
            {
                var user = userDAL.GetByEmail(superAdministrator.Email);
                if (user == null)
                {
                    GlobalThesaurusUser userDb = superAdministrator;
                    userDb.UpdateRoles(roleIds);
                    userDAL.InsertOrUpdate(userDb);
                }
            }
        }

        private List<GlobalThesaurusUser> GetGlobalThesaurusSuperAdministrators()
        {
            return new List<GlobalThesaurusUser>() {
                new GlobalThesaurusUser()
                {
                    Country = "Serbia",
                    FirstName = "Danilo",
                    LastName = "Acimovic",
                    Affiliation = "Developer",
                    Status = GlobalUserStatus.Active,
                    Email = "danilo.acimovic@wemedoo.com",
                    Password = "tptUAfq8sRx4489T",
                    Source = GlobalUserSource.Internal,
                    Phone = ""
                },
                new GlobalThesaurusUser()
                {
                    Country = "Serbia",
                    FirstName = "Luka",
                    LastName = "Jovanovic",
                    Affiliation = "Developer",
                    Status = GlobalUserStatus.Active,
                    Email = "luka.jovanovic@wemedoo.com",
                    Password = "kRKVXuGZ7wPejc5k",
                    Source = GlobalUserSource.Internal,
                    Phone = ""
                },
                new GlobalThesaurusUser()
                {
                    Country = ResourceTypes.CompanyCountry,
                    FirstName = "Nikola",
                    LastName = "Cihoric",
                    Affiliation = "MD",
                    Status = GlobalUserStatus.Active,
                    Email = "nikola.cihoric@wemedoo.com",
                    Password = "JjPHNe7D6tn8GCjE",
                    Source = GlobalUserSource.Internal,
                    Phone = ""
                },
                new GlobalThesaurusUser()
                {
                    Country = ResourceTypes.CompanyCountry,
                    FirstName = "Gorica",
                    LastName = "Bozic",
                    Affiliation = "Tester",
                    Status = GlobalUserStatus.Active,
                    Email = "gorica.bozic@wemedoo.com",
                    Password = "PttVSn4b7Bph7cBu",
                    Source = GlobalUserSource.Internal,
                    Phone = ""
                }
            };
        }

        private void SetGlobalThesaurusOtherUsers(IGlobalThesaurusUserDAL userDAL, List<GlobalThesaurusRole> roles)
        {
            var curator = new GlobalThesaurusUser()
            {
                Country = ResourceTypes.CompanyCountry,
                FirstName = "Dr. Olgun",
                LastName = " Elicin",
                Affiliation = "Curator",
                Status = GlobalUserStatus.Active,
                Email = "olgunelicin@gmail.com",
                Password = "4ywYK3CZvFv9m5WS",
                Source = GlobalUserSource.Internal,
                Phone = ""
            };
            var curatorRole = roles.FirstOrDefault(r => r.Name == PredifinedGlobalUserRole.Curator.ToString());
            
            var user = userDAL.GetByEmail(curator.Email);
            if (user == null && curator != null)
            {
                GlobalThesaurusUser userDb = curator;
                userDb.UpdateRoles(new List<int>(){ curatorRole.GlobalThesaurusRoleId});
                userDAL.InsertOrUpdate(userDb);
            }
        }

        private void PopulateGlobalThesaurusInitialData()
        {
            var thesaurusDAL = DependencyResolver.Current.GetService<IThesaurusDAL>();
            var translationDAL = DependencyResolver.Current.GetService<IThesaurusTranslationDAL>();
            var codeDAL = DependencyResolver.Current.GetService<IO4CodeableConceptDAL>();
            var globalUserDAL = DependencyResolver.Current.GetService<IGlobalThesaurusUserDAL>();
            var administrativeDataDAL = DependencyResolver.Current.GetService<IAdministrativeDataDAL>();

            InsertCodingSystems();

            //SqlImporter thesaurusImporter = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
            //DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
            //DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
            //DependencyResolver.Current.GetService<ICodeSystemDAL>());
            //thesaurusImporter.Import(BlobStorageHelper.GetUrl("UMLS"));

            //SingletonDataContainer.Instance.RefreshSingleton();

            GlobalThesaurusImporter globalThesaurusExcelImporter = new GlobalThesaurusImporter(
                thesaurusDAL, 
                globalUserDAL,
                translationDAL, 
                codeDAL, 
                administrativeDataDAL,
                GlobalThesaurusConstants.FileName,
                GlobalThesaurusConstants.Sheet
             );
            globalThesaurusExcelImporter.ImportDataFromExcelToDatabase();
        }

        #endregion

    }
}
