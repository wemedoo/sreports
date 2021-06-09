using CsvHelper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Initializer.OrganizationCSV;
using sReportsV2.Initializer.PatientJson;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using sReportsV2.Initializer.PredefinedTypes;

namespace sReportsV2.Initializer
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"];
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            var collection = MongoDatabase.GetCollection<EnumEntry>("predefinedtypes") as IMongoCollection<EnumEntry>;
            var thesaurusCollection = MongoDatabase.GetCollection<ThesaurusEntry>("thesaurusentry") as IMongoCollection<ThesaurusEntry>;

            var items = collection.AsQueryable().Where(x => x.Type == "OrganizationIdentifierType").Select(x => x.ThesaurusId).ToList();
            var serviceTypes = thesaurusCollection.AsQueryable().Where(x => items.Contains(x.O40MTId)).Select(x => x.Translations).ToList().Select(x => x.FirstOrDefault(c => c.Language == "en").PreferredTerm).ToList();
            foreach (var term in serviceTypes) 
            {
                Console.WriteLine($"\"{term}\",");
            }


            ExportToCsv(serviceTypes);
        }

        public static void ExportToCsv(List<string> serviceTypes) 
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (var writer = new StreamWriter($@"{desktopPath}\serviceTypes.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    foreach (string serviceType in serviceTypes)
                    {
                        csv.WriteField(serviceType);
                        csv.NextRecord();
                    }
                }
            }
        }
    }
}
