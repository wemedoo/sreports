using System;
using System.Web.Mvc;
using sReportsV2.BusinessLayer.Interfaces;
using System.Net;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.CustomAttributes;
using System.Web;
using sReportsV2.Common.Extensions;
using System.Collections.Generic;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Form.DataOut;
using System.Linq;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Common.Enums;

namespace sReportsV2.Controllers
{
    public class PdfController : BaseController
    {
        private readonly IFormBLL formBLL;
        private readonly IFormInstanceBLL formInstanceBLL;
        private readonly IPdfBLL pdfBLL;
        private readonly IUserBLL userBLL;  

        public PdfController(IFormBLL formBLL, IPdfBLL pdfBLL, IFormInstanceBLL formInstanceBLL, IUserBLL userBLL)
        {
            this.formBLL = formBLL;
            this.formInstanceBLL = formInstanceBLL;
            this.pdfBLL = pdfBLL;
            this.userBLL = userBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult GetPdfForFormId(string formId)
        {
            Form form = formBLL.GetFormById(formId);
            Ensure.IsNotNull(form, nameof(form));

            Byte[] pdfContent = pdfBLL.Generate(form, userCookieData, TranslateDocumentFields(form.Language));
            SetCustomResponseHeaderForMultiFileDownload();
            return File(pdfContent, "application/pdf", form.Title + ".pdf");          
        }
        
        [HttpPost]
        [SReportsAuthorize]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            pdfBLL.Upload(file, userCookieData);

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        private Dictionary<string, string> TranslateDocumentFields(string language)
        {
            return new Dictionary<string, string>()
            {
                { "Language", Resources.TextLanguage.ResourceManager.GetString(language) },
                { "PostingDateTranslation", Resources.TextLanguage.PostingDate },
                { "VersionTranslation", Resources.TextLanguage.Version },
                { "LanguageTranslation", Resources.TextLanguage.Language},
                { "NotesTranslation", Resources.TextLanguage.Notes},
                { "DateTranslation", Resources.TextLanguage.Date}
            };
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult GetSynopticPdf(string formInstanceId)
        {
            FormInstance formInstance = formInstanceBLL.GetById(formInstanceId);
            Ensure.IsNotNull(formInstance, nameof(formInstance));

            List<FormInstance> referrals = formInstanceBLL.GetByIds(formInstance.Referrals ?? new List<string>());
            FormDataOut data = formBLL.GetFormDataOut(formInstance, referrals, userCookieData);

            UserDataOut signingUser = null;
            string signingUserCompleteName= string.Empty;
            if (formInstance.WorkflowHistory != null && formInstance.WorkflowHistory.Last().Status == FormState.Signed)
            {
                signingUser = userBLL.GetById(formInstance.WorkflowHistory.Last().CreatedById);
                signingUserCompleteName= signingUser.FirstName + " " + signingUser.LastName;
            }

            Byte[] pdfContent = pdfBLL.GenerateSynoptic(data, userCookieData, TranslateDocumentFields(formInstance.Language), signingUserCompleteName); 
            return File(pdfContent, "application/pdf", formInstance.Title + ".pdf");
        }
    }
}