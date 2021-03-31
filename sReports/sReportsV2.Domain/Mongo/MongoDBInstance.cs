using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Diagnostics;

namespace sReportsV2.Domain.Mongo
{
    public sealed class MongoDBInstance
    {
        private static volatile MongoDBInstance instance;
        private static readonly object syncLock = new Object();

        private static IMongoDatabase db = null;

        private MongoDBInstance()
        {
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(MongoConfiguration.ConnectionString));
            mongoClientSettings.ClusterConfigurator = cb => {
                cb.Subscribe<CommandStartedEvent>(e => {
                    Debug.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            MongoClient Client = new MongoClient(mongoClientSettings);

            db = Client.GetDatabase("sReports");
        }

        public static MongoDBInstance Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncLock)
                    {
                        if (instance == null)
                            instance = new MongoDBInstance();
                    }
                }

                return instance;
            }
        }

        public IMongoDatabase GetDatabase()
        {
            return db;
        }
    }
}

