using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Common.Entities.User;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormDAL : IFormDAL
    {
        private readonly IMongoCollection<Form> Collection;
        public FormDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Form>("form");
        }

        public bool ExistsForm(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).CountDocuments() > 0;
        }

        public List<Form> GetAll(FormFilterData filterData)
        {
            IQueryable<Form> result = this.GetFormsFiltered(filterData)
                .OrderByDescending(x => x.EntryDatetime);
            if (filterData != null)
            {
                result = result
                    .Skip((filterData.Page - 1) * filterData.PageSize)
                    .Take(filterData.PageSize);
            }
            return result.ToList();
        }

        public List<FormInstancePerDomain> GetFormInstancePerDomain()
        {
            var result = Collection.Aggregate()
                                   .Unwind<Form,FormOverridden>(x => x.DocumentProperties.ClinicalDomain)
                                   .Project(x => new FormInstancePerDomain()
                                   {
                                       Domain = x.DocumentProperties.ClinicalDomain,
                                       Count = x.DocumentsCount


                                   })
                                   .ToList()
                                   .GroupBy(x => x.Domain)
                                    .Select(x => new FormInstancePerDomain()
                                    {
                                        Domain = x.Key,
                                        Count = x.Sum(s => s.Count)

                                    })
                                    .ToList();
            return result.Where(x => x.Count != 0).ToList();
        }

        public Form GetForm(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted);
        }

        public Form GetFormByThesaurus(int thesaurusId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted);
        }

        public Form GetFormByThesaurusAndVersion(int thesaurusId,string versionId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted && x.Version.Id == versionId);
        }

        public Form GetFormByThesaurusAndLanguage(int thesaurusId, string language)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && x.Language.Equals(language) && !x.IsDeleted);
        }

        public bool Delete(string formId, DateTime lastUpdate)
        {
            Form formForDelete = GetForm(formId);
            Entity.DoConcurrencyCheckForDelete(formForDelete);
            formForDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<Form>.Filter.Eq(x => x.Id, formId);
            var update = Builders<Form>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<Form> GetDocumentsByThesaurusAppeareance(int o4mtId)
        {
            return GetThesaurusAppereance(o4mtId).ToList();
        }

        public List<Form> GetFilteredDocumentsByThesaurusAppeareance(int o4mtId, string searchTerm, int thesaurusPageNum)
        {
            return GetThesaurusAppereance(o4mtId, searchTerm)
                  .Skip(thesaurusPageNum).Take(15).ToList();
        }

        public long GetThesaurusAppereanceCount(int o4mtId, string searchTerm)
        {
            return GetThesaurusAppereance(o4mtId, searchTerm).Count();
        }

        public Form InsertOrUpdate(Form form, UserData user, bool updateVersion = true)
        {
            form = Ensure.IsNotNull(form, nameof(form));
            user = Ensure.IsNotNull(user, nameof(user));

            if (string.IsNullOrEmpty(form.Id))
            {
                form.EntryDatetime = DateTime.Now;
                form.LastUpdate = DateTime.Now;
                form.Version.Id = updateVersion ? Guid.NewGuid().ToString() : form.Version.Id;
                form.SetWorkflow(user, form.State);

                Collection.InsertOne(form);
            }
            else
            {
                Form formForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(form.Id));
                formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);               
                form.EntryDatetime = formForUpdate.EntryDatetime;
                form.LastUpdate = DateTime.Now;
                form.WorkflowHistory = formForUpdate.WorkflowHistory ?? new List<FormStatus>();
                form.SetWorkflow(user, form.State);

                form.DocumentsCount = formForUpdate.DocumentsCount;
                var filter = Builders<Form>.Filter.Where(s => s.Id.Equals(form.Id));
                Collection.ReplaceOne(filter, form);
            }

            return form;
        }

        public long GetAllFormsCount(FormFilterData filterData)
        {
            return this.GetFormsFiltered(filterData).Count();
        }

        public bool ExistsFormByThesaurus(int thesaurusId)
        {
            return Collection
                .Find(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted)
                .CountDocuments() > 0;
        }

        public Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Id.Equals(versionId)
                    && x.OrganizationId == organizationId)
                .FirstOrDefault();
        }

        public long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(string thesaurusId, int organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Major.Equals(version.Major)
                    && x.Version.Id != version.Id 
                    && x.Version.Minor.Equals(version.Minor)
                    && x.OrganizationId == organizationId)
                .CountDocuments();
        }

        public DocumentProperties GetDocumentProperties(string id)
        {
            return Collection
                .AsQueryable()
                .FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted)
                ?.DocumentProperties;

        }

        public List<Form> GetByEntryDatetime(DateTime entryDatetime)
        {
            return Collection.AsQueryable().Where(x => x.EntryDatetime >= entryDatetime).ToList();
        }

        public List<Form> GetDistinctThesaurus(FormFilterData filterData)
        {

            return Collection.AsQueryable().ToList();
        }

        public List<Form> GetAllByOrganization(int organization, int limit, int offset)
        {
            return Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.OrganizationId == organization)
                .OrderBy(x => x.Title)
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public List<Form> GetAllByOrganizationAndLanguage(int organization, string language)
        {
            return Collection
                .AsQueryable()
                .Where(x => !x.IsDeleted)
                .Where(x => x.OrganizationId == organization)
                .Where(x => x.Language == language)
                .Where(x => x.State == FormDefinitionState.ReadyForDataCapture)
                .OrderBy(x => x.Title).
                Select(x => new Form(){
                    Id = x.Id,
                    Title = x.Title,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                }).ToList(); ;
        }

        public Task<List<Form>> GetAllByOrganizationAndLanguageAsync(int organization, string language)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.OrganizationId == organization
                    && x.Language == language
                    && x.State == FormDefinitionState.ReadyForDataCapture)
                .SortBy(x => x.Title)
                .Project(x => new Form()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                })
                .ToListAsync();
        }

        public List<Form> GetAllByOrganizationAndLanguageAndName(int organization, string language, string name)
        {
            return Collection
                .AsQueryable()
                .Where(x => !x.IsDeleted)
                .Where(x => x.OrganizationId == organization)
                .Where(x => x.Language == language)
                .Where(x => x.State == FormDefinitionState.ReadyForDataCapture)
                .Where(x => string.IsNullOrEmpty(name) || x.Title.ToLower().StartsWith(name.ToLower()))
                .OrderBy(x => x.Title).
                Select(x => new Form()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                })
                .ToList();
        }

        public Task<List<Form>> GetAllByOrganizationAndLanguageAndNameAsync(int organization, string language, string name)
        {
            return Collection
                .Find(x => !x.IsDeleted 
                    && x.OrganizationId == organization
                    && x.Language == language
                    && x.State == FormDefinitionState.ReadyForDataCapture
                    && string.IsNullOrEmpty(name) || x.Title.ToLower().StartsWith(name.ToLower()))
                .SortBy(x => x.Title)
                .Project(x => new Form()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                })
                .ToListAsync();
        }

        public int CountByOrganization(int organization)
        {
            return Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.OrganizationId == organization)
                .Count();
        }

        public bool ExistsFormByThesaurusAndLanguage(string thesaurusId, string language)
        {
            return Collection.AsQueryable().Any(x => !x.IsDeleted && x.ThesaurusId.Equals(thesaurusId) && x.Language.Equals(language));
        }

        public List<Form> GetFormsByThesaurusAndLanguageAndOrganization(string thesaurus, int organizationId, string activeLanguage)
        {
            return Collection.AsQueryable()
                           .Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationId == organizationId).Select(x => new Form() {Id = x.Id , Version = x.Version })
                           .ToList();
        }

        public Form GetFormWithGreatestVersion(string thesaurusId, int activeOrganization, string activeLanguage)
        {
            List<Form> forms = this.GetFormsByThesaurusAndLanguageAndOrganization(thesaurusId, activeOrganization, activeLanguage);
            Form result =forms != null && forms.Count > 0 ? forms[0] : null;
            foreach (Form form in forms)
            {
                if (form.Version.IsVersionGreater(result.Version))
                {
                    result = form;
                }
            }

            return result;
        }

        public List<Form> GetManyLanguageAndThesaurusList(string language, List<int> thesaurusList)
        {
            return Collection.AsQueryable().Where(x => thesaurusList.Contains(x.ThesaurusId) && x.Language.Equals(language)).ToList();
        }

        public void DisableFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage)
        {
            var filter = Builders<Form>.Filter.Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationId == organizationId);

            var update =Builders<Form>.Update.Set(x => x.State, FormDefinitionState.Archive);

            Collection.UpdateMany(filter, update);
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return GetDocumentsByThesaurusAppeareance(thesaurusId).Count > 1;
        }

        public List<Form> GetByFormIdsList(List<string> ids)
        {
            return Collection.Find(x => ids.Contains(x.Id)).ToList();
        }

        public Task<List<Form>> GetByFormIdsListAsync(List<string> ids)
        {
            return  Collection.Find(x => ids.Contains(x.Id)).ToListAsync();
        }

        public List<string> GetByClinicalDomains(List<DocumentClinicalDomain> clinicalDomains)
        {
            return Collection.AsQueryable()
                             .Where(x => x.DocumentProperties.ClinicalDomain.Any(c => clinicalDomains.Contains(c.Value)))
                             .Select(f => f.Id)
                             .Distinct()
                             .ToList();
        }

        private IEnumerable<Form> GetThesaurusAppereance(int o4mtId, string searchTerm = null)
        {
            //var filter = Builders<Form>.Filter.Where(x => x.ThesaurusId == o4mtId
            //|| x.Chapters.Any(c => c.ThesaurusId == o4mtId 
            //    || c.Pages.Any(p => p.ThesaurusId == o4mtId || p.ListOfFieldSets.Any(lfs => lfs.Any(fs => fs.ThesaurusId == o4mtId)))));

            var filter = Builders<Form>.Filter.Eq("ThesaurusId", o4mtId) |
                         Builders<Form>.Filter.Eq("Chapters.ThesaurusId", o4mtId) |
                         Builders<Form>.Filter.Eq("Chapters.Pages.ThesaurusId", o4mtId) |
                         Builders<Form>.Filter.Eq("Chapters.Pages.ListOfFieldSets.0.ThesaurusId", o4mtId) |
                         Builders<Form>.Filter.Eq("Chapters.Pages.ListOfFieldSets.0.Fields.ThesaurusId", o4mtId);

            return Collection.Find(filter).ToList();

        }

        private IQueryable<Form> GetFormsFiltered(FormFilterData filterData)
        {
            IQueryable<Form> result = Collection.AsQueryable(new AggregateOptions() { AllowDiskUse = true }).Where(x => !x.IsDeleted /*&& x.State.Equals(Enums.FormDefinitionState.ReadyForDataCapture)*/);
            if (filterData != null)
            {
                result = result.Where(x => x.Language != null && x.Language.Equals(filterData.ActiveLanguage)
                        && x.OrganizationId == filterData.OrganizationId
                        && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                        && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class != null && x.DocumentProperties.Class.Class == filterData.Classes))
                        && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                        && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain.Count > 0 && x.DocumentProperties.ClinicalDomain.Contains(filterData.ClinicalDomain)))
                        && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                        && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                        && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                        && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                        && (filterData.State == null || x.State == filterData.State)
                        && (filterData.ThesaurusId == 0 || (x.ThesaurusId == filterData.ThesaurusId))
                        && (filterData.DateTimeTo == null || x.EntryDatetime < filterData.DateTimeTo)
                        && (filterData.DateTimeFrom == null || x.EntryDatetime > filterData.DateTimeFrom)

                   );
                var test = result.ToList();
                if (!string.IsNullOrWhiteSpace(filterData.Title))
                {
                    result = result.Where(x => x.Title.ToLower().StartsWith(filterData.Title.ToLower()));
                }

            }

            return result;
        }

        
    }
}
