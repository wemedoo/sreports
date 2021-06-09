using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Implementations;

using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.UMLS.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UMLSClient.Client;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Initializer.OrganizationCSV;
using sReportsV2.Initializer.PredefinedTypes;

namespace sReportsV2.Controllers
{
    public class ImportDataController : Controller
    {
        IThesaurusDAL thesaurusDAL;
        IThesaurusEntryBLL thesaurusBLL;
        ICodeSystemDAL codeSystemDAL;
        public ImportDataController(IThesaurusDAL thesaurusDAL, IOrganizationBLL organizationBLL, IThesaurusEntryBLL thesaurusBLL, ICodeSystemDAL codeSystemDAL)
        {
            this.thesaurusBLL = thesaurusBLL;
            this.thesaurusDAL = thesaurusDAL;
            this.codeSystemDAL = codeSystemDAL;
        }

        public ActionResult InsertOrganizations()
        {

            ImportOrganization importer = new ImportOrganization(DependencyResolver.Current.GetService<IOrganizationDAL>());
            importer.InsertOrganization(AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\SwissHospitals.csv");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult InsertPredefinedTypesSql()
        {

            PredefineTypesImporter importer = new PredefineTypesImporter(DependencyResolver.Current.GetService<ICustomEnumDAL>(), DependencyResolver.Current.GetService<IThesaurusDAL>());
            importer.Import();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult InsertCodingSystems()
        {
            if (codeSystemDAL.GetAllCount() > 0) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            SqlImporter importer = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
                DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
                DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
                DependencyResolver.Current.GetService<ICodeSystemDAL>());
            importer.ImportCodingSystems(BlobStorageHelper.GetUrl("UMLS"));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult InsertThesaurusesIntoSql()
        {
            if (thesaurusDAL.GetAllCount() > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }


            SqlImporter importer = new SqlImporter(DependencyResolver.Current.GetService<IThesaurusDAL>(),
                DependencyResolver.Current.GetService<IThesaurusTranslationDAL>(),
                DependencyResolver.Current.GetService<IO4CodeableConceptDAL>(),
                DependencyResolver.Current.GetService<ICodeSystemDAL>());
            importer.Import(BlobStorageHelper.GetUrl("UMLS"));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        /* private ThesaurusService thesaurusService;

         public ImportDataController()
         {
             thesaurusService = new ThesaurusService();
         }

         public ActionResult ImportLoinc() {
             LoincParser.LoincParser.LoincParser parser = new LoincParser.LoincParser.LoincParser();
             parser.ImportThesaurusesAndFormsFromLoinc(BlobStorageHelper.GetUrl("loinc"));

             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }
         public async Task InsertThesaurusEntriesFromUMLS()
         {
             Importer importer = new Importer();
             await Task.Run(() =>
             {
                 importer.Import(BlobStorageHelper.GetUrl("UMLS"));

             }).ConfigureAwait(false);
         }

         public  ActionResult InsertThesaurusesIntoSql()
         {
             if (thesaurusService.GetAllCount() > 0)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.Conflict);
             }
             CodeSystemService service = new CodeSystemService();
             var codingSystems = service.GetAll(); 

             SqlImporter importer = new SqlImporter();
             importer.Import(BlobStorageHelper.GetUrl("UMLS"));
             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }

         public ActionResult Test()
         {
             //List<ThesaurusEntry> thesauruses = thesaurusService
             var thesaurus = thesaurusService.GetById(493317);
             thesaurusService.Insert(new ThesaurusEntry()
             {
                 Translations = new List<ThesaurusEntryTranslation>()
                 {
                     new ThesaurusEntryTranslation()
                     {
                         Synonyms = new List<string>()
                         {
                             "s1",
                             "s2",
                             "s3"
                         },
                         Abbreviations =  new List<string>()
                         {
                             "a1",
                             "a2",
                             "a3"
                         }
                     }

                 }
             });

             return new HttpStatusCodeResult(HttpStatusCode.OK);
         }

         public ActionResult ExportToCsv()
         {
             CsvExporter exporter = new CsvExporter();
             exporter.SetTermsCsv(System.IO.File.ReadAllLines($"{AppDomain.CurrentDomain.GetData("DataDirectory").ToString()}/terms.txt").ToList());
             exporter.Import(BlobStorageHelper.GetUrl("UMLS"));
             var sb = new StringBuilder();


             sb.AppendLine("Ui, Name, Definition, Atoms, Semantic Types, Term");
             foreach (var csvEntity in exporter.GetCsvEntities())
             {
                 sb.AppendLine(csvEntity.UI + "," + "\"" + csvEntity.Name + "\"" + "," + "\"" + csvEntity.Definition + "\"" + "," + "\"" + csvEntity.Atoms + "\"" + "," + "\"" + csvEntity.SemanticType + "\"" + "," + "\"" + csvEntity.Term + "\"");
             } 

             return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv");
         }

         public string GetAtomsAsString(List<string> atoms) 
         {
             string result = string.Empty;
             foreach (string atom in atoms) 
             {
                 result += atom + "\n";
             }

             return result;
         }

         public void InsertIdentifiers()
         {
             IdentifierInitializer initializer = new IdentifierInitializer();
             initializer.InitializeIdentifiers(initializer.GetOrganizationIdentifierList(), IdentifierKind.OrganizationIdentifierType);
             initializer.InitializeIdentifiers(initializer.GetPatientIdentifierList(), IdentifierKind.PatientIdentifierType);
         }

         public async Task InsertPatients()
         {
             ImportPatient importPatient = new ImportPatient();
             WebClient webClient = new WebClient();
             webClient.Encoding = Encoding.UTF8;

             await Task.Run(() =>
                 {
                     foreach (string patientUrl in BlobStorageHelper.GetUrls("fhirPatients"))
                         importPatient.LoadFromJson(webClient.DownloadString(patientUrl), importPatient.userData);
                 }).ConfigureAwait(false);

         }

         public void InsertOrganizations() 
         {
             ImportOrganization importOrganization = new ImportOrganization();
             importOrganization.InsertOrganization(AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\SwissHospitals.csv");
         } 
         */
    }
}