using MongoDB.Driver;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.MongoMigrationVersion;
using sReportsV2.Domain.Mongo;
using System;
using System.Configuration;
using System.IO;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public class MongoMigrator
    {
        private readonly IMongoCollection<MongoMigrationVersion> Collection;
            
        public MongoMigrator()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<MongoMigrationVersion>("migrationversion");
            // Creating a Unique Index for Version Field (if doesn't already exists)
            IndexKeysDefinition<MongoMigrationVersion> key = Builders<MongoMigrationVersion>.IndexKeys.Ascending("Version");
            Collection.Indexes.CreateOne(new CreateIndexModel<MongoMigrationVersion>(key, new CreateIndexOptions() { Name = "VersionUniqueIndex", Unique = true }));
        }

        /// <summary>
        ///     Maps Each Migration Version with correspective object.
        ///     Note: Every time a new Migration is created, MUST be added in a new case and in the LastVersion case.
        /// </summary>
        private static MongoMigration VersionToMigration(int version)
        {
            switch (version)
            {
                case 1:
                    return new M_202211221536_ValueLabel();
                case 2:
                    return new M_202212081410_DateFormat();
                case MongoMigrationsConstants.LastVersion:
                    return new M_202212081410_DateFormat();
                default:
                    return null ;
            }
        }

        public void SetToLatestVersion()
        {
            SaveMigrationsVersionFromJsonToDB();  // Temporary Helper Method. TODO : Remove when Obsolete

            int currentVersion = LoadLastSavedMigrationVersion();
            int lastSavedVersion = VersionToMigration(MongoMigrationsConstants.LastVersion).Version;
          
            if (currentVersion < lastSavedVersion)
                UpgradeMigrationVersion(currentVersion, lastSavedVersion);
        }

        public void SetToVersion(int desiredVersion)
        {
            int currentVersion = LoadLastSavedMigrationVersion();

            if (desiredVersion >= currentVersion)
                UpgradeMigrationVersion(currentVersion, desiredVersion);
            else
                DowngradeMigrationVersion(currentVersion, desiredVersion);
        }

        private void UpgradeMigrationVersion(int currentVersion, int desiredVersion)
        {
            for (int i = currentVersion +1; i <= desiredVersion; i++)
            {
                MongoMigration migration = VersionToMigration(i);
                if (migration is null) throw new NullReferenceException($"Migration version {i} is null");
                
                migration.Up();
                SaveMigrationVersion(migration.Version, migration.GetType().Name);
            }
        }

        private void DowngradeMigrationVersion(int currentVersion, int desiredVersion)
        {
            for (int i = currentVersion; i > desiredVersion; i--)
            {
                MongoMigration migration = VersionToMigration(i);
                if (migration is null) throw new NullReferenceException($"Migration version {i} is null");
                
                migration.Down();
                SaveMigrationVersion(migration.Version - 1, migration.GetType().Name);
            }
        }

        public void SaveMigrationVersion(int migrationVersion, string name)
        {
            MongoMigrationVersion migrationToSave = new MongoMigrationVersion() { 
                Version = migrationVersion, 
                Name = name,
                EntryDatetime = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            Collection.InsertOne(migrationToSave);
        }

        public int LoadLastSavedMigrationVersion()
        {
            MongoMigrationVersion lastSavedMigration = Collection.Aggregate().SortByDescending(x => x.Version).Limit(1).FirstOrDefault();

            if (lastSavedMigration != null && lastSavedMigration.Version > 0)
                return lastSavedMigration.Version;

            return 0;
        }

        // Temporary Helper Method. TODO : Remove when Obsolete
        public void SaveMigrationsVersionFromJsonToDB()
        {
            string pathAndFilename = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["MongoMigrationVersionFile"]);

            if (File.Exists(pathAndFilename))
            {
                string json = File.ReadAllText(pathAndFilename);
                int version = JsonConvert.DeserializeObject<int>(json);

                // If a Migration with the same Version already exists, MongoWriteException is thrown (because Version is a Unique index)
                try
                {
                    if (version >= 1)
                        SaveMigrationVersion(1, "M_202211221536_ValueLabel");
                }
                catch(MongoWriteException){}

                try
                {
                    if (version == 2)
                        SaveMigrationVersion(2, "M_202212081410_DateFormat"); 
                }
                catch (MongoWriteException) {}
            }
        }
    }
}
