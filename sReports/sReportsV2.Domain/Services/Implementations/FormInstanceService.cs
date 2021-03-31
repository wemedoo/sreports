using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.UserEntities;
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
    public class FormInstanceService : IFormInstanceService
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly IMongoCollection<EncounterEntity> CollectionEncounter;
        private readonly IMongoCollection<Form> CollectionForm;
        private readonly IMongoCollection<User> CollectionUser;
        private readonly IMongoCollection<PatientEntity> CollectionPatient;



        public FormInstanceService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FormInstance>("forminstance") as IMongoCollection<FormInstance>;
            CollectionEncounter = MongoDatabase.GetCollection<EncounterEntity>("encounterentity") as IMongoCollection<EncounterEntity>;
            CollectionForm = MongoDatabase.GetCollection<Form>("form") as IMongoCollection<Form>;
            CollectionUser = MongoDatabase.GetCollection<User>("users") as IMongoCollection<User>;
            CollectionPatient = MongoDatabase.GetCollection<PatientEntity>("patiententity") as IMongoCollection<PatientEntity>;

        }

        public FormInstance GetById(string id)
        {
            return Collection.Find(x =>!x.IsDeleted && x.Id.Equals(id)).FirstOrDefault();
        }

        public string InsertOrUpdate(FormInstance form)
        {
            form = Ensure.IsNotNull(form, nameof(form));

            if (form.Id == null)
            {
                form.EntryDatetime = DateTime.Now;
                form.LastUpdate = DateTime.Now;
                Collection.InsertOne(form);
            }
            else
            {
                FormInstance formForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(form.Id));
                //formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);
                
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
            DoConcurrencyCheckForDelete(formInstanceForDelete);
            //formInstanceForDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<FormInstance>.Filter.Eq(x => x.Id, id);
            var update = Builders<FormInstance>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<FormInstance> GetByFormThesaurusId(FormInstanceFilterData filterData)
        {
            filterData = Ensure.IsNotNull(filterData, nameof(filterData));

            List<FormInstance> result = this.GetFormInstancesFiltered(filterData)
                .OrderByDescending(x => x.LastUpdate)
                .Skip((filterData.Page-1) * filterData.PageSize)
                .Take(filterData.PageSize)
                .GroupJoin(
                      CollectionUser.AsQueryable(),
                       p => p.UserRef,
                       o => o.Id,
                       (p, o) => new FormInstance {
                           Id = p.Id,
                           EntryDatetime = p.EntryDatetime,
                           LastUpdate = p.LastUpdate,
                           Version = p.Version,
                           Language = p.Language,
                           PatientRef = p.PatientRef,
                           User = o.First() 
                       }
                 )
                .GroupJoin(
                      CollectionPatient.AsQueryable(),
                       p => p.PatientRef,
                       o => o.Id,
                       (p, o) => new FormInstance {
                           Id = p.Id,
                           EntryDatetime = p.EntryDatetime,
                           LastUpdate = p.LastUpdate,
                           Version = p.Version,
                           Language = p.Language,
                           User = p.User,
                           PatientRef = p.PatientRef,
                           Patient = o.First()
                       }
                 )
                .ToList();

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
            return Collection.AsQueryable().Where(x => x.Fields.Any(y => y.ThesaurusId.Equals(thesaurusId) && y.Value.Equals(value))).ToList();
        }

        public List<FormInstance> GetByAllByDefinitionId(string id)
        {
            return Collection.AsQueryable().Where(x => x.FormDefinitionId.Equals(id)).ToList();
        }

        public int CountByDefinition(string id)
        {
            return Collection.AsQueryable().Where(x => x.FormDefinitionId.Equals(id)).Count();
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
            return Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).CountDocuments() > 0;
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
            return Collection.AsQueryable().Where(x => ids.Contains(x.Id));
        }

        public List<FormInstance> GetByEpisodeOfCare(string episodeOfCareId, int PageSize, int Page)
        {
            List<string> encounters = CollectionEncounter
                .AsQueryable()
                .Where(x => x.EpisodeOfCareId.Equals(episodeOfCareId))
                .Select(x => x.Id).ToList();

            return Collection
                .AsQueryable()
                .Where(x => encounters.Contains(x.EncounterRef) && !x.IsDeleted)
                .GroupJoin(
                      CollectionUser.AsQueryable(),
                       p => p.UserRef,
                       o => o.Id,
                       (p, o) => new FormInstance
                       {
                           Id = p.Id,
                           EntryDatetime = p.EntryDatetime,
                           Title = p.Title,
                           LastUpdate = p.LastUpdate,
                           Version = p.Version,
                           OrganizationRef = p.OrganizationRef,
                           Language = p.Language,
                           User = o.First()
                       }
                 )
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public List<FormInstance> GetByEpisodeOfCareId(string episodeOfCareId)
        {
            List<string> encounters = CollectionEncounter
                .AsQueryable()
                .Where(x => x.EpisodeOfCareId.Equals(episodeOfCareId))
                .Select(x => x.Id).ToList();

            return Collection
                .AsQueryable()
                .Where(x => encounters.Contains(x.EncounterRef))
                .ToList();
        }

        public int CountEpisodeOfCare(string episodeOfCareId)
        {
            List<string> encounters = CollectionEncounter
                .AsQueryable()
                .Where(x =>x.EpisodeOfCareId != null && x.EpisodeOfCareId.Equals(episodeOfCareId))
                .Select(x => x.Id).ToList();

            return Collection
                .AsQueryable()
                .Where(x => encounters.Contains(x.Id)).Count();
        }

        public bool ExistsById(string id)
        {
            return Collection.AsQueryable().Any(x => x.Id == id);
        }

        private IQueryable<FormInstance> GetFormInstancesFiltered(FormInstanceFilterData filterData)
        {
            return Collection
                .AsQueryable()
                .Where(x =>!x.IsDeleted && x.ThesaurusId.Equals(filterData.ThesaurusId)/* && x.Version.Id.Equals(filterData.VersionId)*/
                    && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                    && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class == filterData.Classes))
                    && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                    && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain == filterData.ClinicalDomain))
                    && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                    && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null &&  x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                    && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null &&  x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                    && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                );
        }

        public List<FormInstance> GetByParameters(FormInstanceFhirFilter formInstancesFilter)
        {

            return Collection.Find(x => (formInstancesFilter.Encounter == null || x.EncounterRef.Equals(formInstancesFilter.Encounter))
                                            && (formInstancesFilter.Performer == null || x.UserRef.Equals(formInstancesFilter.Performer))
                                            && !x.IsDeleted).ToList();
        }

        public List<FormInstancePerDomain> GetFormInstancePerDomain()
        {
            //List<DocumentClinicalDomain> thesaurusIds = Enum.GetValues(typeof(DocumentClinicalDomain)).Cast<DocumentClinicalDomain>().ToList();
            var result = Collection.AsQueryable()
                .Where(x => x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain != null)
                .GroupBy(x => x.DocumentProperties.ClinicalDomain)
                .Select(x => new FormInstancePerDomain()
                {
                    Domain = x.Key,
                    Count = x.Count()
                })
                .ToList();

            /*thesaurusIds.Except(result.Select(x => x.ThesaurusId)).ToList().ForEach(x =>
            {
                result.Add(new FormInstancePerThesaurus()
                {
                    Count = 0,
                    ThesaurusId = x
                });
            });*/
            return result;
        }
        private void DoConcurrencyCheckForDelete(FormInstance forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
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
            Collection.InsertMany(formInstances);
        }
    }
}
