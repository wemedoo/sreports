using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormService : IFormService
    {
        //private readonly MongoHelper<Form> Forms;
        private readonly IMongoCollection<Form> Collection;
        public FormService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Form>("form") as IMongoCollection<Form>;
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

        public Form GetForm(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted);
        }

        public Form GetFormByThesaurus(string thesaurusId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted);
        }

        public Form GetFormByThesaurusAndLanguage(string thesaurusId, string language)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && x.Language.Equals(language) && !x.IsDeleted);
        }

        public bool Delete(string formId, DateTime lastUpdate)
        {
            Form formForDelete = GetForm(formId);
            DoConcurrencyCheckForDelete(formForDelete);
            formForDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<Form>.Filter.Eq(x => x.Id, formId);
            var update = Builders<Form>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<Form> GetDocumentsByThesaurusAppeareance(string o4mtId)
        {
            return Collection
                .AsQueryable()
                .Where(form => !form.IsDeleted &&
                    form.Chapters.Any(chapter =>
                        chapter.Pages.Any(page =>
                            page.ListOfFieldSets.Any(listOfFS =>
                                listOfFS.Any(
                                    fieldSet => fieldSet.Fields.Any(field =>
                                        field.ThesaurusId.Equals(o4mtId) || (((FieldSelectable)field).Values != null && ((FieldSelectable)field).Values.Any(value =>value.ThesaurusId.Equals(o4mtId)))) 
                                    || fieldSet.ThesaurusId.Equals(o4mtId))
                               )
                            || page.ThesaurusId.Equals(o4mtId))
                        || chapter.ThesaurusId.Equals(o4mtId))
                    || form.ThesaurusId.Equals(o4mtId)).ToList();
        }

        public void InsertOrUpdate(Form form, UserData user, bool updateVersion = true)
        {
            form = Ensure.IsNotNull(form, nameof(form));
            user = Ensure.IsNotNull(user, nameof(user));

            FormDefinitionState state = form.State;

            if (string.IsNullOrEmpty(form.Id))
            {
                form.EntryDatetime = DateTime.Now;
                form.LastUpdate = DateTime.Now;
                form.WorkflowHistory.Add(new FormStatus()
                {
                    Created = DateTime.Now,
                    Status = state,
                    User = user.Id
                });
                form.Version.Id = updateVersion ? Guid.NewGuid().ToString() : form.Version.Id;
                Collection.InsertOne(form);
            }
            else
            {
                Form formForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(form.Id));
                formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);
               
                form.EntryDatetime = formForUpdate.EntryDatetime;
                form.LastUpdate = DateTime.Now;
                form.WorkflowHistory = formForUpdate.WorkflowHistory ?? new List<FormStatus>();
                form.SetWorkflow(user, state);
                var filter = Builders<Form>.Filter.Where(s => s.Id.Equals(form.Id));
                Collection.ReplaceOne(filter, form);
            }
        }

        public long GetAllFormsCount(FormFilterData filterData)
        {
            return this.GetFormsFiltered(filterData).Count();
        }

        public bool ExistsFormByThesaurus(string thesaurusId)
        {
            return Collection
                .Find(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted)
                .CountDocuments() > 0;
        }

        public Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(string thesaurusId, string organizationId, string activeLanguage, string versionId)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Id.Equals(versionId)
                    && x.OrganizationRef.Equals(organizationId))
                .FirstOrDefault();
        }

        public long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(string thesaurusId, string organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Major.Equals(version.Major)
                    && x.Version.Id != version.Id 
                    && x.Version.Minor.Equals(version.Minor)
                    && x.OrganizationRef.Equals(organizationId))
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

        public List<Form> GetAllByOrganizationAndLanguage(string organization, string language)
        {
            return Collection.AsQueryable().Where(x => x.OrganizationRef == organization && x.Language == language && !x.IsDeleted && x.State == FormDefinitionState.ReadyForDataCapture)
                .OrderBy(x => x.Title).
                Select(x => new Form(){
                    Id = x.Id,
                    Title = x.Title,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                }).ToList(); ;
        }

        public List<Form> GetAllByOrganization(string organization, int limit, int offset)
        {
            return Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.OrganizationRef.Equals(organization))
                .OrderBy(x => x.Title)
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public int CountByOrganization(string organization)
        {
            return Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.OrganizationRef.Equals(organization))
                .Count();
        }

        public bool ExistsFormByThesaurusAndLanguage(string thesaurusId, string language)
        {
            return Collection.AsQueryable().Any(x => !x.IsDeleted && x.ThesaurusId.Equals(thesaurusId) && x.Language.Equals(language));
        }

        private IQueryable<Form> GetFormsFiltered(FormFilterData filterData)
        {
            IQueryable<Form> result = Collection.AsQueryable().Where(x => !x.IsDeleted /*&& x.State.Equals(Enums.FormDefinitionState.ReadyForDataCapture)*/);
            if (filterData != null)
            {
                result = result.Where(x => x.Language != null && x.Language.Equals(filterData.ActiveLanguage)
                        && x.OrganizationRef.Equals(filterData.OrganizationId)
                        && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                        && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class == filterData.Classes))
                        && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                        && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain == filterData.ClinicalDomain))
                        && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                        && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                        && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                        && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                        && (filterData.State == null || (x.State == filterData.State))
                   );
            }

            return result;
        }

        private void DoConcurrencyCheckForDelete(Form forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }

        public List<Form> GetFormsByThesaurusAndLanguageAndOrganization(string thesaurus, string organizationId, string activeLanguage)
        {
            return Collection.AsQueryable()
                           .Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationRef.Equals(organizationId)).Select(x => new Form() {Id = x.Id , Version = x.Version })
                           .ToList();
        }

        public Form GetFormWithGreatestVersion(string thesaurusId, string activeOrganization, string activeLanguage)
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

        public List<Form> GetManyLanguageAndThesaurusList(string language, List<string> thesaurusList)
        {
            return Collection.AsQueryable().Where(x => thesaurusList.Contains(x.ThesaurusId) && x.Language.Equals(language)).ToList();
        }

        public void DisableFormsByThesaurusAndLanguageAndOrganization(string thesaurus, string organizationId, string activeLanguage)
        {
            var filter = Builders<Form>.Filter.Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationRef.Equals(organizationId));

            var update =Builders<Form>.Update.Set(x => x.State, FormDefinitionState.Disabled);

            Collection.UpdateMany(filter, update);
        }
    }
}
