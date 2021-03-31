using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormService
    {
        List<Form> GetAll(FormFilterData filterData);
        List<Form> GetAllByOrganizationAndLanguage(string organization, string language);
        List<Form> GetDistinctThesaurus(FormFilterData filterData);
        Form GetForm(string formId);
        Form GetFormByThesaurus(string thesaurusId);
        Form GetFormByThesaurusAndLanguage(string thesaurusId, string language);
        Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(string thesaurusId, string organizationId, string activeLanguage, string versionId);
        bool Delete(string formId, DateTime lastUpdate);
        bool ExistsForm(string formId);
        bool ExistsFormByThesaurusAndLanguage(string thesaurusId, string language);
        bool ExistsFormByThesaurus(string thesaurusId);
        void InsertOrUpdate(Form form, UserData user, bool updateVersion = true);
        long GetAllFormsCount(FormFilterData filterData);
        List<Form> GetDocumentsByThesaurusAppeareance(string o4mtId);
        List<Form> GetByEntryDatetime(DateTime entryDatetime);
        DocumentProperties GetDocumentProperties(string formId);
        long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(string thesaurusId, string organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version);
        List<Form> GetFormsByThesaurusAndLanguageAndOrganization(string thesaurus, string organizationId, string activeLanguage);
        Form GetFormWithGreatestVersion(string thesaurusId, string activeOrganization, string activeLanguage);
        List<Form> GetManyLanguageAndThesaurusList(string language, List<string> thesaurusList);
        List<Form> GetAllByOrganization(string organization, int limit, int offset);
        int CountByOrganization(string organization);
        void DisableFormsByThesaurusAndLanguageAndOrganization(string thesaurus, string organizationId, string activeLanguage);

    }
}
