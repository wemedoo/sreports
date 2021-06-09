using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class ImportSettingsService : IImportSettingsService
    {
        private readonly IMongoCollection<ImportSettings> Collection;

        public ImportSettingsService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<ImportSettings>("importsettings") as IMongoCollection<ImportSettings>;
        }

        public bool ExistSettings(string type, int version)
        {
            return Collection.Find(x => x.Version == version && x.Type == type).CountDocuments() > 0;
        }

        public void InsertOne(ImportSettings settings)
        {
            settings = Ensure.IsNotNull(settings, nameof(settings));

            settings.EntryDatetime = DateTime.Now;
            settings.IsDeleted = false;
            settings.LastUpdate = settings.EntryDatetime;
            Collection.InsertOne(settings);
        }
    }
}
