using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFormBLL
    {
        PaginationDataOut<FormDataOut, FormFilterDataIn> ReloadData(FormFilterDataIn dataIn, UserCookieData userCookieData);
        Form GetFormByThesaurusAndLanguage(int thesaurusId, string language);
        FormDataOut GetFormDataOut(Form form, UserCookieData userCookieData);
        Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId);
        List<FormInstancePerDomainDataOut> GetFormInstancePerDomain();
        FormDataOut SetFormDependablesAndReferrals(Form form, List<Form> referrals);
        Form GetForm(FormInstance formInstance, UserCookieData userCookieData);
        FormDataOut GetFormDataOut(FormInstance formInstance, List<FormInstance> referrals, UserCookieData userCookieData);
        List<Form> GetFormsFromReferrals(List<FormInstance> referrals);
        Form GetFormById(string formId);
        TreeDataOut GetTreeDataOut(int thesaurusId, int thesaurusPageNum, string searchTerm);
        bool TryGenerateNewLanguage(string formId, string language, UserCookieData userCookieData);
        void DisableActiveFormsIfNewVersion(Form form, UserCookieData userCookieData);
    }
}
