using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
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
                .Find(x =>!x.IsDeleted && x.Id.Equals(id))
                .FirstOrDefault();
        }

        public string InsertOrUpdate(FormInstance form)
        {
            form = Ensure.IsNotNull(form, nameof(form));

            if (form.Id == null)
            {
                form.EntryDatetime = DateTime.Now;
                form.LastUpdate = DateTime.Now;
                Collection.InsertOne(form);
                IncrementFormDocumentCount(form.FormDefinitionId, 1);
            }
            else
            {
                FormInstance formForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(form.Id));
                formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);           
                form.EntryDatetime = formForUpdate.EntryDatetime;
                form.LastUpdate = DateTime.Now;
                var filter = Builders<FormInstance>.Filter.Eq(s => s.Id, form.Id);
                var result = Collection.ReplaceOne(filter,form).ModifiedCount;
            }

            return form.Id;
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            FormInstance formInstanceForDelete = GetById(id);
            Entity.DoConcurrencyCheckForDelete(formInstanceForDelete);
            //formInstanceForDelete.DoConcurrencyCheck(lastUpdate);

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
                .Skip((filterData.Page - 1)*filterData.PageSize)
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

        public List<FormInstance> GetFormsByFieldThesaurusAndValue(string thesaurusId, string value)
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
                .Where(x => x.FormDefinitionId.Equals(id))
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
        public List<FormInstance> GetByEpisodeOfCareId(string episodeOfCareId)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.EpisodeOfCareRef.Equals(episodeOfCareId))
                .ToList();
        }

        public Task<List<FormInstance>> GetAllByEpisodeOfCareIdAsync(string episodeOfCareId)
        {
            return Collection.Find(x => x.EpisodeOfCareRef.Equals(episodeOfCareId)).ToListAsync();
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
                                            && (string.IsNullOrWhiteSpace(filter.ThesaurusId)  || x.ThesaurusId.Equals(filter.ThesaurusId))
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

        public bool ExistThesaurus(int thesaurusId)
        {
            return Collection.AsQueryable()
                            .Where(instance => !instance.IsDeleted &&
                                instance.Fields.Any(field =>
                                    field.ThesaurusId == thesaurusId)
                                || instance.ThesaurusId.Equals(thesaurusId)).Count() > 1;
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

        public List<FormInstance> GetAllByEncounter(int encounterId)
        {
            return this.Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId))
                .Select(x => new FormInstance()
                {
                    Id = x.Id,
                    ThesaurusId = x.ThesaurusId,
                    FormDefinitionId = x.FormDefinitionId,
                    Title = x.Title
                })
                .ToList();
        }

        public Task<List<FormInstance>> GetAllByEncounterAsync(int encounterId)
        {
            return this.Collection.AsQueryable().Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId))
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

        private IQueryable<FormInstance> GetFormInstancesFiltered(FormInstanceFilterData filterData)
        {
            return Collection
                .AsQueryable()
                .Where(x => !x.IsDeleted && x.ThesaurusId.Equals(filterData.ThesaurusId)/* && x.Version.Id.Equals(filterData.VersionId)*/
                    && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                    && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class != null && x.DocumentProperties.Class.Class == filterData.Classes))
                    && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                    && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain.Count > 0 && x.DocumentProperties.ClinicalDomain.Contains(filterData.ClinicalDomain)))
                    && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                    && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                    && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                    && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                );
        }

        private void IncrementFormDocumentCount(string formDefinitionId, int value)
        {
            var filterForm = Builders<Form>.Filter.Eq(f => f.Id, formDefinitionId);
            var updateForm = Builders<Form>.Update.Inc(f => f.DocumentsCount, value);
            CollectionForm.FindOneAndUpdate(filterForm, updateForm);
        }
    }
}
