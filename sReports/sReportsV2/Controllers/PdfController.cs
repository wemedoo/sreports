using System;
using System.Web.Mvc;
using sReportsV2.BusinessLayer.Interfaces;
using System.Net;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.CustomAttributes;
using System.Web;
using sReportsV2.Common.Extensions;
using System.Collections.Generic;

namespace sReportsV2.Controllers
{
    public class PdfController : BaseController
    {
        private readonly IFormBLL formBLL;
        private readonly IPdfBLL pdfBLL;

        public PdfController(IFormBLL formBLL, IPdfBLL pdfBLL)
        {
            this.formBLL = formBLL;
            this.pdfBLL = pdfBLL;
        }

        public ActionResult GetPdfForFormId(string formId)
        {
            Form form = formBLL.GetFormById(formId);
            Ensure.IsNotNull(form, nameof(form));

            Byte[] pdfContent = pdfBLL.Generate(form, userCookieData, TranslateDocumentFields(form.Language));

            return File(pdfContent, "application/pdf", form.Title + ".pdf");          
        }
        
        [HttpPost]
        [SReportsAutorize]
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
    }
}