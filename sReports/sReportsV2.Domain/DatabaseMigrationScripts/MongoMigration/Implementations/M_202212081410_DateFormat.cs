using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{

    public class M_202212081410_DateFormat : MongoMigration
    {
        private IMongoCollection<FormInstance> Collection;
        public override int Version => 2;

        public M_202212081410_DateFormat()
        {
            Collection = MongoDBInstance.Instance.GetDatabase().GetCollection<FormInstance>("forminstance");
        }

        public override void Up()
        {
            string dateFormatRegexToUpdate = ".{4}-.{2}-.{2}";
            UpdateFieldsDateFormat(dateFormatRegexToUpdate, DateConstants.DateFormat).Wait();
        }

        public override void Down()
        {
            string dateFormatRegexToUpdate = ".{2}/.{2}/.{4}";
            UpdateFieldsDateFormat(dateFormatRegexToUpdate, "yyyy-MM-dd").Wait();
        }

        // ---------- Helper Methods ----------

        private async Task UpdateFieldsDateFormat(string dateFormatRegexToUpdate, string updatedDateFormatRegex)
        {
            var fieldFilters = Builders<FieldValue>.Filter.Eq(x => x.Type, FieldTypes.Date)
                        & Builders<FieldValue>.Filter.Regex(x => x.ValueLabel, new BsonRegularExpression(dateFormatRegexToUpdate));
            var findOptions = new FindOptions<FormInstance, FormInstance>() { BatchSize = 5000 };
            var instancesToWrite = new List<WriteModel<FormInstance>>();
            // filter : matches docs with DATE fields where their Value matches the Regex expression
            var filter = Builders<FormInstance>.Filter.ElemMatch(x => x.Fields, fieldFilters);

            using (var cursor = await Collection.FindAsync(filter, findOptions))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;                   
                    

                    batch.SelectMany(forminstance => forminstance.Fields)
                        .Where(fieldValue => fieldValue.Type == FieldTypes.Date).ToList()
                        .ForEach(fieldValue => {
                            if (fieldValue.ValueLabel != null)
                            {
                                for (int i = 0; i < fieldValue.ValueLabel.Count; i++)
                                {
                                    if (Regex.Match(fieldValue.ValueLabel[i], dateFormatRegexToUpdate).Success)
                                    {
                                        if (DateTime.TryParse(fieldValue.ValueLabel[i], out DateTime date))
                                        {
                                            fieldValue.ValueLabel[i] = date.ToString(updatedDateFormatRegex);
                                            fieldValue.Value[i] = date.ToString(updatedDateFormatRegex);
                                        }
                                                
                                    }
                                }
                            }
                        });
                    
                    //Replacing the modified FormInstances into MongoDB Collection
                    foreach (FormInstance formInstance in batch)
                    {
                        var replaceFilter = Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id);
                        instancesToWrite.Add(new ReplaceOneModel<FormInstance>(replaceFilter, formInstance));
                    }

                    var result = await Collection.BulkWriteAsync(instancesToWrite);
                    if (!result.IsAcknowledged)
                        throw new InvalidOperationException($"BulkWriteAsync wrote {result.InsertedCount} items instead of {batch.Count()}");

                    instancesToWrite.Clear();
                }
            }
        }
    }
}
