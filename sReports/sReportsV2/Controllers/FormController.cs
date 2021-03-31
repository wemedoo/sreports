using AutoMapper;
using Generator;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.Models.Form.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace sReportsV2.Controllers
{
    [Authorize]
    public class FormController : FormCommonController
    {
        public IEnumService enumService;
        public IOrganizationService organizationService;
        public IUserService userService;
        public ILogger Logger;
        public IThesaurusEntryService thesaurus;
        public FormController()
        {
            enumService = new EnumService();
            organizationService = new OrganizationService();
            userService = new UserService();
            thesaurus = new ThesaurusEntryService();
        }


        [SReportsAuditLog]
        [Authorize]
        public ActionResult GetAll(FormFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDataOut>>>(this.enumService.GetDocumentPropertiesEnums());
            return PartialView("FormsTable", ReloadData(dataIn));
        }

        [SReportsAutorize]
        public ActionResult ReloadByFormThesaurusTable(FormFilterDataIn dataIn)
        {
            dataIn.State = Domain.Enums.FormDefinitionState.ReadyForDataCapture;
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDataOut>>>(this.enumService.GetDocumentPropertiesEnums());
            return PartialView("~/Views/FormInstance/FormsTable.cshtml", ReloadData(dataIn));
        }

        [SReportsAuditLog]
        [Authorize]
        public ActionResult Get(string thesaurusId)
        {
            Form form = this.formService.GetFormByThesaurusAndLanguage(thesaurusId, userCookieData.ActiveLanguage);
            if (form == null)
            {
                NotFound(thesaurusId);
            }

            return View(GetFormDataOut(form));
        }

        [SReportsAuditLog]
        [Authorize]
        public ActionResult Edit(string thesaurusId, string versionId)
        {
            Form form = this.formService.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId,userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, versionId);
            if (form == null)
            {
                NotFound(thesaurusId);
            }

            return View(GetFormDataOut(form));
        }

        [SReportsAuditLog]
        [Authorize]
        public ActionResult CreateForm()
        {
            return View("Edit", new FormDataOut());
        }

        [Authorize]
        public ActionResult GetDocumentsByThesaurusId(string o4MtId)
        {
            var forms = formService.GetDocumentsByThesaurusAppeareance(o4MtId);
            TreeViewModel result = new TreeViewModel()
            {
                Forms = Mapper.Map<List<FormTreeViewModel>>(forms),
                O4MtId = o4MtId
            };
            return PartialView("FormTree", result);
        }

        [Authorize]
        public ActionResult GetDocumentProperties(string id)
        {
            DocumentProperties result = this.formService.GetDocumentProperties(id);
            return PartialView("DocumentProperties", Mapper.Map<DocumentPropertiesDataOut>(result));
        }

        [SReportsAuditLog]
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [SReportsFormValidate]
        public ActionResult Create([ModelBinder(typeof(JsonNetModelBinder))]  FormDataIn formDataIn, string formId)
        {
            var tst = formDataIn.GetAllFields().FirstOrDefault(x => x.Type == FieldTypes.Radio);
            if (formId != null && !formService.ExistsForm(formId))
            {
                Log.Warning(SReportsResource.FormNotExists, 404, formId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            formDataIn = Ensure.IsNotNull(formDataIn, nameof(formDataIn));

            Form form = Mapper.Map<Form>(formDataIn);
            UserData userData = Mapper.Map<UserData>(userCookieData);

            form.UserRef = userCookieData.Id;
            form.OrganizationRef = userCookieData.GetActiveOrganizationData()?.Id;
            form.Language = userCookieData.ActiveLanguage;

            if (!string.IsNullOrWhiteSpace(formId)) 
            {
                Form formFromDatabase = formService.GetForm(formId);
                if (form.IsVersionChanged(formFromDatabase))
                {
                    form.Id = null;
                    form.Version.Id = Guid.NewGuid().ToString();
                    //set all common form state to disabled
                    formService.DisableFormsByThesaurusAndLanguageAndOrganization(formFromDatabase.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
                }
            }
            try
            {
                formService.InsertOrUpdate(form, userData);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuditLog]
        [HttpDelete]
        [Authorize]
        public ActionResult Delete(string formId, DateTime lastUpdate)
        {
            try
            {
                formService.Delete(formId, lastUpdate);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDeleteEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }
            catch (MongoDbConcurrencyDeleteException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDelete;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }


        public ActionResult ExportCTCAEForm()
        {
            FormGenerator generator = new FormGenerator(Mapper.Map<UserData>(userCookieData));
            Form form = generator.GetFormFromCsv("CTCAE ");
            formService.InsertOrUpdate(form, Mapper.Map<UserData>(userCookieData));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult GenerateNewLanguage(string formid, string language)
        {
            Form form = formService.GetForm(formid);
            if (form == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            List<string> thesaurusList = form.GetAllThesaurusIds();
            UserData userData = Mapper.Map<UserData>(userCookieData);
            List<ThesaurusEntry> entries = thesaurus.GetByIdsList(thesaurusList);
            if (entries.Count.Equals(0))
            {
                form.Id = null;
                form.Language = language;
                formService.InsertOrUpdate(form, userData,false);
            }
            else
            {
                form.Id = null;
                form.GenerateTranslation(entries, language);
                formService.InsertOrUpdate(form, userData, false);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        public ActionResult GenerateThesauruses(string formId)
        {
            Form form = formService.GetForm(formId);
            UserData userData = Mapper.Map<UserData>(userCookieData);
            ThesaurusGenerator generator = new ThesaurusGenerator();
            generator.GenerateThesauruses(form, Mapper.Map<UserData>(userCookieData));
            formService.InsertOrUpdate(form, userData);

            return RedirectToAction("Edit", "Form", new { formId });
        }

        private FormFilterData GetFormFilterData(FormFilterDataIn formDataIn)
        {
            FormFilterData result = Mapper.Map<FormFilterData>(formDataIn);
            result.OrganizationId = userCookieData.GetActiveOrganizationData()?.Id;
            result.ActiveLanguage = userCookieData.ActiveLanguage;
            return result;
        }

        private PaginationDataOut<FormDataOut, FormFilterDataIn> ReloadData(FormFilterDataIn dataIn)
        {
            FormFilterData filterData = GetFormFilterData(dataIn);
            PaginationDataOut<FormDataOut, FormFilterDataIn> result = new PaginationDataOut<FormDataOut, FormFilterDataIn>()
            {
                Count = (int)this.formService.GetAllFormsCount(filterData),
                Data = Mapper.Map<List<FormDataOut>>(this.formService.GetAll(filterData)),
                DataIn = dataIn
            };
            return result;
        }

        private FormDataOut GetFormDataOut(Form form)
        {
            FormDataOut dataOut = Mapper.Map<FormDataOut>(form);
            dataOut.User = Mapper.Map<UserDataDataOut>(userCookieData);
            dataOut.Organization = Mapper.Map<OrganizationDataOut>(this.organizationService.GetOrganizationById(form.OrganizationRef));
            if (form.WorkflowHistory != null)
            {
                dataOut.WorkflowHistory = new List<FormStatusDataOut>();
                foreach (FormStatus status in form.WorkflowHistory)
                {
                    dataOut.WorkflowHistory.Add(new FormStatusDataOut()
                    {
                        Created = status.Created,
                        Status = status.Status,
                        User = Mapper.Map<UserDataDataOut>(userService.GetById(status.User))
                    });
                }
            }
            return dataOut;
        }        
    }
}