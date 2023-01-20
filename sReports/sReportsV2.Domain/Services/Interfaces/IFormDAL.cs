using MongoDB.Bson;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormDAL
    {
        List<Form> GetAll(FormFilterData filterData);
        List<Form> GetAllByOrganizationAndLanguage(int organization, string language);
        Task<List<Form>> GetAllByOrganizationAndLanguageAsync(int organization, string language);
        List<Form> GetAllByOrganizationAndLanguageAndName(int organization, string language, string name);
        Task<List<Form>> GetAllByOrganizationAndLanguageAndNameAsync(int organization, string language, string name);
        Form GetForm(string formId);
        Form GetFormByThesaurus(int thesaurusId);
        Form GetFormByThesaurusAndVersion(int thesaurusId, string versionId);
        List<FormInstancePerDomain> GetFormInstancePerDomain();
        Form GetFormByThesaurusAndLanguage(int thesaurusId, string language);
        Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId);
        bool Delete(string formId, DateTime lastUpdate);
        bool ExistsForm(string formId);
        bool ExistsFormByThesaurusAndLanguage(int thesaurusId, string language);
        bool ExistsFormByThesaurus(int thesaurusId);
        Form InsertOrUpdate(Form form, UserData user, bool updateVersion = true);
        long GetAllFormsCount(FormFilterData filterData);
        List<Form> GetFilteredDocumentsByThesaurusAppeareance(int o4mtId, string searchTerm, int thesaurusPageNum, int? organizationId);
        long GetThesaurusAppereanceCount(int o4mtId, string searchTerm, int? organizationId = null);
        List<Form> GetByEntryDatetime(DateTime entryDatetime);
        DocumentProperties GetDocumentProperties(string formId);
        long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(int thesaurusId, int organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version);
        List<Form> GetFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage);
        Form GetFormWithGreatestVersion(int thesaurusId, int activeOrganization, string activeLanguage);
        List<Form> GetManyLanguageAndThesaurusList(string language, List<int> thesaurusList);
        List<Form> GetAllByOrganization(int organization, int limit, int offset);
        List<Form> GetByFormIdsList(List<string> ids);
        Task<List<Form>> GetByFormIdsListAsync(List<string> id);
        int CountByOrganization(int organization);
        void DisableFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage);
        bool ThesaurusExist(int thesaurusId);
        List<string> GetByClinicalDomains(List<DocumentClinicalDomain> clinicalDomains);
        List<Form> GetAllWithEmptyThesaurusIdsList();
        List<BsonDocument> GetPlottableFields(string formId);
    }
}
