using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Organization;
using sReportsV2.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Common.CustomAttributes
{
    public class SReportsFormValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext = Ensure.IsNotNull(filterContext, nameof(filterContext));

            FormDataIn formDataIn = filterContext.ActionParameters["formDataIn"] as FormDataIn;

            if (!IsFormValid(formDataIn))
            {
               filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Conflict, "Form with same thesaurus, language, organization and version alredy exist!");
            }

            if (!IsVersionValid(formDataIn)) 
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Conflict, $"New version of document should be greater than {GetGretestVersion(formDataIn)}!");
            }


            List<string> listDuplicateIds = formDataIn.ValidateFieldsIds();
            if (listDuplicateIds.Count != 0)
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Conflict, Resources.TextLanguage.DuplicateFieldIds + ": " + string.Join(", ", listDuplicateIds));

        }

        private bool IsFormValid(FormDataIn formDataIn)
        {
            UserCookieData user = System.Web.HttpContext.Current.Session.GetUserFromSession();
            FormService formService = new FormService();
            long formCount = formService.GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(formDataIn.ThesaurusId, user.ActiveOrganization, formDataIn.Language, formDataIn.Version);
            
            return formCount > 0 ? false : true;
        }

        private bool IsVersionValid(FormDataIn formDataIn)
        {
            UserCookieData user = System.Web.HttpContext.Current.Session.GetUserFromSession();
            FormService formService = new FormService();
            Form formWithGreatestVersion = formService.GetFormWithGreatestVersion(formDataIn.ThesaurusId, user.ActiveOrganization, user.ActiveLanguage);

            if (formWithGreatestVersion == null || (formWithGreatestVersion.Id == formDataIn.Id && formDataIn.Version.Major == formWithGreatestVersion.Version.Major && formDataIn.Version.Minor == formWithGreatestVersion.Version.Minor)) 
            {
                return true;
            }
            return formDataIn.Version.IsVersionGreater(formWithGreatestVersion.Version);
        }

        private string GetGretestVersion(FormDataIn formDataIn)
        {
            UserCookieData user = System.Web.HttpContext.Current.Session.GetUserFromSession();
            FormService formService = new FormService();
            Form form = formService.GetFormWithGreatestVersion(formDataIn.ThesaurusId, user.ActiveOrganization, user.ActiveLanguage);

            return $"{form.Version.Major}.{form.Version.Minor}";
        }

    }
}