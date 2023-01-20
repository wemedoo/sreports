using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FieldFilters;
using sReportsV2.Domain.FieldFilters.Implementations;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormInstanceDAL : IFormInstanceDAL
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly IMongoCollection<EncounterEntity> CollectionEncounter;
        private readonly IMongoCollection<Form> CollectionForm;
        private readonly IMongoCollection<PatientEntity> CollectionPatient;




        public FormInstanceDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FormInstance>("forminstance");
            CollectionEncounter = MongoDatabase.GetCollection<EncounterEntity>("encounterentity");
            CollectionForm = MongoDatabase.GetCollection<Form>("form");
            CollectionPatient = MongoDatabase.GetCollection<PatientEntity>("patiententity");
        }

        public FormInstance GetById(string id)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(id))
                .FirstOrDefault();
        }

        public string InsertOrUpdate(FormInstance formInstance, FormInstanceStatus formInstanceStatus)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            if (formInstance.Id == null)
            {
                formInstance.Copy(null, formInstanceStatus);
                Collection.InsertOne(formInstance);
                IncrementFormDocumentCount(formInstance.FormDefinitionId, 1);
            }
            else
            {
                FormInstance formInstanceDb = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formInstance.Id));
                formInstanceDb.DoConcurrencyCheck(formInstance.LastUpdate.Value);
                formInstance.Copy(formInstanceDb, formInstanceStatus);
                var filter = Builders<FormInstance>.Filter.Eq(s => s.Id, formInstance.Id);
                var result = Collection.ReplaceOne(filter,formInstance).ModifiedCount;
            }

            return formInstance.Id;
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            FormInstance formInstanceForDelete = GetById(id);
            Entity.DoConcurrencyCheckForDelete(formInstanceForDelete);
            formInstanceForDelete.DoConcurrencyBeforeDeleteCheck(lastUpdate);

            var filter = Builders<FormInstance>.Filter.Eq(x => x.Id, id);
            var update = Builders<FormInstance>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            IncrementFormDocumentCount(formInstanceForDelete.FormDefinitionId, -1);

            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<FormInstance> GetByFormThesaurusId(FormInstanceFilterData filterData)
        {
            filterData = Ensure.IsNotNull(filterData, nameof(filterData));

            List<FormInstance> result = this.GetFormInstancesFiltered(filterData)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filterData.Page - 1) * filterData.PageSize)
                .Take(filterData.PageSize)
                .ToList();
            // TO DO IMPLEMENT THE METHOD

            return result;
        }

        public List<FormInstance> GetAllFormsByFieldIdAndValue(string id, string value)
        {
            return Collection.AsQueryable()
                .Where(x => x.Fields.Any(y => y.Id.Equals(id) && y.Value.Equals(value)))
                .ToList();
        }

        public List<FormInstance> GetFormsByFieldThesaurusAndValue(int thesaurusId, string value)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.Fields.Any(y => y.ThesaurusId.Equals(thesaurusId) && y.Value.Equals(value)))
                .ToList();
        }

        public List<FormInstance> GetByAllByDefinitionId(string id)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.FormDefinitionId.Equals(id) && !x.IsDeleted)
                .ToList();
        }

        public int CountByDefinition(string id)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.FormDefinitionId.Equals(id))
                .Count();
        }

        public List<FormInstance> GetByDefinitionId(string id, int limit, int offset)
        {
            return Collection.AsQueryable()
                .Where(x => x.FormDefinitionId.Equals(id))
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public bool ExistsFormInstance(string id)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(id))
                .CountDocuments() > 0;
        }

        public long GetAllInstancesByThesaurusCount(FormInstanceFilterData filterData)
        {
            return GetFormInstancesFiltered(filterData).Count();
        }

        public IQueryable<FormInstance> GetByIds(List<string> ids)
        {
            if (ids == null)
            {
                ids = new List<string>();
            }
            //TO DO IMPLEMENT THE METHOD
            return Collection.AsQueryable().Where(x => ids.Contains(x.Id));

        }
        public List<FormInstance> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.OrganizationId == organizationId)
                .ToList();
        }

        public Task<List<FormInstance>> GetAllByEpisodeOfCareIdAsync(int episodeOfCareId, int organizationId)
        {
            return Collection.Find(x => x.EpisodeOfCareRef == episodeOfCareId && x.OrganizationId == organizationId).ToListAsync();
        }

        public bool ExistsById(string id)
        {
            return Collection.AsQueryable().Any(x => x.Id == id);
        }

        public List<FormInstance> GetByParameters(FormInstanceFhirFilter formInstancesFilter)
        {

            return Collection.Find(x => (formInstancesFilter.Encounter == null || x.EncounterRef.Equals(formInstancesFilter.Encounter))
                                            && (formInstancesFilter.Performer == null || x.UserId == formInstancesFilter.Performer)
                                            && !x.IsDeleted).ToList();
        }

        public List<FormInstance> GetAllByCovidFilter(FormInstanceCovidFilter filter)
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted
                                            && (filter.LastUpdate == null || x.LastUpdate > filter.LastUpdate)
                                            && (x.ThesaurusId.Equals(filter.ThesaurusId))
                                            ).ToList();
        }

        public async Task<List<FormInstance>> GetAllFieldsByCovidFilter()
        {
            FindOptions options = new FindOptions
            {
                BatchSize = 1000,
                NoCursorTimeout = false
            };
            var filter = Builders<FormInstance>.Filter.Eq("ThesaurusId", "14573");
            var projection = Builders<FormInstance>.Projection
                .Include("Chapters.Pages.FieldSets.Fields.Label")
                .Include("Chapters.Pages.FieldSets.Fields.Type")
                .Include("Chapters.Pages.FieldSets.Fields.Value")
                .Include("Chapters.Pages.FieldSets.Fields.ThesaurusId")
                .Include("Chapters.Pages.FieldSets.Fields.Values");
            var result = await Collection.Find(filter, options).Project<FormInstance>(projection).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public void InsertMany(List<FormInstance> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            IncrementFormDocumentCount(formInstances.FirstOrDefault().FormDefinitionId, formInstances.Count);
            Collection.InsertMany(formInstances);
        }

        public List<FormInstance> GetAll()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).ToList();
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return Collection.AsQueryable()
                            .Any(instance => !instance.IsDeleted &&
                                instance.Fields.Any(field =>
                                    field.ThesaurusId == thesaurusId)
                                || instance.ThesaurusId.Equals(thesaurusId));
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            var filter = Builders<FormInstance>.Filter;
            var formInstanceAndFieldThesaurusId = filter.And(
            filter.ElemMatch(x => x.Fields, c => c.ThesaurusId == oldThesaurus));

            var update = Builders<FormInstance>.Update;
            var fieldsLevelSetter = update.Set("Fields.$.ThesaurusId", newThesaurus);
            Collection.UpdateMany(formInstanceAndFieldThesaurusId, fieldsLevelSetter);

            var filterThesaurus = Builders<FormInstance>.Filter.Eq("ThesaurusId", oldThesaurus);

            var updateThesaurus = Builders<FormInstance>.Update;
            var formInstanceSetter = update.Set("ThesaurusId", newThesaurus);
            Collection.UpdateMany(filterThesaurus, formInstanceSetter);

        }

        public List<FormInstance> GetAllByEncounter(int encounterId, int organizationId)
        {
            return this.Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId) && x.OrganizationId == organizationId)
                .Select(x => new FormInstance()
                {
                    Id = x.Id,
                    ThesaurusId = x.ThesaurusId,
                    FormDefinitionId = x.FormDefinitionId,
                    Title = x.Title
                })
                .ToList();
        }

        public Task<List<FormInstance>> GetAllByEncounterAsync(int encounterId, int organizationId)
        {
            return this.Collection.AsQueryable().Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId) && x.OrganizationId == organizationId)
                .Select(x => new FormInstance()
                {
                    Id = x.Id,
                    ThesaurusId = x.ThesaurusId,
                    FormDefinitionId = x.FormDefinitionId,
                    Title = x.Title,
                    EntryDatetime = x.EntryDatetime
                })
                .ToListAsync();
        }


        public List<FormInstance> SearchByTitle(int episodeOfCareId, string title)
        {
            return Collection.AsQueryable().Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.Title.ToUpper().Contains(title.ToUpper()))
               .OrderByDescending(x => x.EntryDatetime)
               .ToList();
        }

        private IQueryable<FormInstance> GetFormInstancesFiltered(FormInstanceFilterData filterData)
        {
            IQueryable<FormInstance> result = Collection.AsQueryable(new AggregateOptions() { AllowDiskUse = true });

            // Text Search needs to be first pipeline step
            if (!string.IsNullOrWhiteSpace(filterData.Content))
            {
                filterData.Content = filterData.Content.RemoveDiacritics().ToLower();
                result = TextSearch(result, filterData.Content);
            }

            result = result.Where(x => !x.IsDeleted && x.ThesaurusId.Equals(filterData.ThesaurusId) && x.Version.Id.Equals(filterData.VersionId)
                    && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                    && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class != null && x.DocumentProperties.Class.Class == filterData.Classes))
                    && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                    && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain.Count > 0 && x.DocumentProperties.ClinicalDomain.Contains(filterData.ClinicalDomain)))
                    && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                    && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                    && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                    && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                );            

            if (filterData.UserIds != null && filterData.UserIds.Count != 0)
            {
                FilterDefinition<FormInstance> userFilter = 
                    new BsonDocument("UserId", new BsonDocument("$in", new BsonArray(filterData.UserIds)));
                result = result.Where(x => userFilter.Inject());
            }            

            if (filterData.PatientIds != null && filterData.PatientIds.Count != 0)
            {
                FilterDefinition<FormInstance> patientFilter =
                    new BsonDocument("PatientId", new BsonDocument("$in", new BsonArray(filterData.PatientIds)));
                result = result.Where(x => patientFilter.Inject());
            }

            if(filterData.CustomFieldFiltersData != null && filterData.CustomFieldFiltersData.Count > 0 && filterData.FieldFiltersOverallOperator != null)
            {
                result = GetByCustomFieldFilters(result, filterData);
            }

            return result.Select(x => new FormInstance()
            {
                Title = x.Title,
                Version = x.Version,
                EntryDatetime = x.EntryDatetime,
                LastUpdate = x.LastUpdate,
                PatientId = x.PatientId,
                Language = x.Language,
                OrganizationId = x.OrganizationId,
                UserId = x.UserId,
                Id = x.Id
            }); 
        }

        public List<BsonDocument> GetPlottableFieldsByThesaurusId(string formId, int thesaurusId)
        {
            var stage1 = new BsonDocument("FormDefinitionId", formId);
            var stage2 =
                    new BsonDocument
                        {
                            { "Date", 1 },
                            { "EntryDatetime", 1 },
                            { "Fields",
                        new BsonDocument("$filter",
                        new BsonDocument
                                    {
                                        { "input", "$Fields" },
                                        { "as", "field" },
                                        { "cond",
                        new BsonDocument("$eq",
                        new BsonArray
                                            {
                                                "$$field.ThesaurusId",
                                                thesaurusId
                                            }) }
                                    }) }
                        };
            var stage3 =
                    new BsonDocument
                        {
                            { "Value",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Fields.Value",
                                    0
                                }) },
                            { "Date", 1 },
                            { "EntryDateTimeValue", "$EntryDatetime.DateTime" }
                        };

            var aggregationResult = Collection.Aggregate()
                .Match(stage1)
                .Project(stage2)
                .Project(stage3).ToList();

            return aggregationResult;
        }

        private IQueryable<FormInstance> GetByCustomFieldFilters(IQueryable<FormInstance> formInstancesQueryableCollection, FormInstanceFilterData filterData)
        {
            List<CustomFieldFilter> fieldFilters = new List<CustomFieldFilter>();
            foreach (CustomFieldFilterData f in filterData.CustomFieldFiltersData)
                fieldFilters.Add(CustomFieldFiltersFactory.Create(f));

            FilterDefinition<FormInstance> compoundFilter;
            switch (filterData.FieldFiltersOverallOperator)
            {
                case LogicalOperators.OR:
                    {
                        compoundFilter = fieldFilters.AllOr();
                        break;
                    }
                case LogicalOperators.AND:
                    {
                        compoundFilter = fieldFilters.AllAnd();
                        break;
                    }
                default:
                    {
                        compoundFilter = fieldFilters.AllOr();
                        break;
                    }
            }
            return formInstancesQueryableCollection.Where(x => x.FormDefinitionId == filterData.FormId && compoundFilter.Inject());
        }

        private void IncrementFormDocumentCount(string formDefinitionId, int value)
        {
            var filterForm = Builders<Form>.Filter.Eq(f => f.Id, formDefinitionId);
            var updateForm = Builders<Form>.Update.Inc(f => f.DocumentsCount, value);
            CollectionForm.FindOneAndUpdate(filterForm, updateForm);
        }

        private IQueryable<FormInstance> TextSearch(IQueryable<FormInstance> formInstancesQueryableCollection, string content)
        {
            FilterDefinition<FormInstance> contentFilter = Builders<FormInstance>.Filter.Text(content.PrepareForMongoStrictTextSearch());
            return formInstancesQueryableCollection.Where(x => contentFilter.Inject());
        }
    }
}
