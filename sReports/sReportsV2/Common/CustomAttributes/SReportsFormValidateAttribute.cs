using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DTO;
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

            if (!string.IsNullOrWhiteSpace(ValidationSummary(formDataIn)))
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Conflict, ValidationSummary(formDataIn));
            }

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
            FormDAL formService = new FormDAL();
            long formCount = formService.GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(formDataIn.ThesaurusId, user.ActiveOrganization, formDataIn.Language, formDataIn.Version);
            
            return formCount > 0 ? false : true;
        }

        private bool IsVersionValid(FormDataIn formDataIn)
        {
            UserCookieData user = System.Web.HttpContext.Current.Session.GetUserFromSession();
            FormDAL formService = new FormDAL();
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
            FormDAL formService = new FormDAL();
            Form form = formService.GetFormWithGreatestVersion(formDataIn.ThesaurusId, user.ActiveOrganization, user.ActiveLanguage);

            return $"{form.Version.Major}.{form.Version.Minor}";
        }

        private string ValidationSummary(FormDataIn formDataIn)
        {
            string  result = string.IsNullOrWhiteSpace(formDataIn.ThesaurusId) ? "Form has no thesaurus!" : string.Empty;
            result +=  ValidateChapters(formDataIn.Chapters);

            return result;
        }

        private string ValidateChapters(List<FormChapterDataIn> chapters)
        {
            string result = "";
            foreach(FormChapterDataIn chapter in chapters) 
            {
                result += string.IsNullOrWhiteSpace(chapter.ThesaurusId) ? $"Chapter({chapter.Title}) has no thesaurus!</br>" : string.Empty;
                result += ValidatePages(chapter.Pages);
            }
            return result;
        }
        private string ValidatePages(List<FormPageDataIn> pages)
        {
            string result = "";
            int i = 0;
            foreach (FormPageDataIn page in pages)
            {
                i++;
                result += string.IsNullOrWhiteSpace(page.ThesaurusId) ? $"Page({page.Title}) has no thesaurus!</br>" : string.Empty;
                result +=ValidateFieldSets(page.ListOfFieldSets);

            }
            return result;
        }

        private string ValidateFieldSets(List<List<FormFieldSetDataIn>> listOfFs)
        {
            string result = "";
            foreach(FormFieldSetDataIn fieldset in listOfFs.SelectMany(x => x).Select(y => y).ToList())
            {
                result += string.IsNullOrWhiteSpace(fieldset.ThesaurusId) ? $"FieldSet({fieldset.Label}) has no thesaurus!</br>" : string.Empty;
                result += ValidateFields(fieldset.Fields);

            }
            return result;
        }

        private string ValidateFields(List<FieldDataIn> fields)
        {
            string result = "";
            foreach (FieldDataIn field in fields) 
            {
                result += string.IsNullOrWhiteSpace(field.ThesaurusId) ? $"Field ({field.Label}) has no thesaurus! </br>" : string.Empty;
                if (field is FieldSelectableDataIn) 
                {
                    result += ValidateFieldValues((field as FieldSelectableDataIn).Values);
                }
            }
            return result;
        }

        private string ValidateFieldValues(List<FormFieldValueDataIn> values)
        {
            string result = "";
            foreach (FormFieldValueDataIn value in values)
            {
                result += string.IsNullOrWhiteSpace(value.ThesaurusId) ? $"Field value ({value.Label}) has no thesaurus!</br>" : string.Empty;
            }
            return result;
        }

    }
}