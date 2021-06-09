using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.Form.DTO;
using sReportsV2.DTOs.Pagination;
using sReportsV2.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.BusinessLayer.Interfaces;
using Generator;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Entities.User;

namespace sReportsV2.Controllers
{
    [SReportsAutorize]
    public partial class FormController : FormCommonController
    {

        public ILogger Logger;
        private ICommentBLL commentBLL;
        private readonly IConsensusDAL consensusDAL;
        public FormController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IConsensusDAL consensusDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL, ICommentBLL commentBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL ,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
            this.customEnumBLL = customEnumBLL;
            this.commentBLL = commentBLL;
            this.consensusDAL = consensusDAL;
        }


        [SReportsAuditLog]

        public ActionResult GetAll(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            return PartialView("FormsTable", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAutorize]
        public ActionResult ReloadByFormThesaurusTable(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            //dataIn.State = Domain.Enums.FormDefinitionState.ReadyForDataCapture;
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            return PartialView("~/Views/FormInstance/FormsTable.cshtml", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult Get(int thesaurusId)
        {
            Form form = this.formBLL.GetFormByThesaurusAndLanguage(thesaurusId, userCookieData.ActiveLanguage);
            if (form == null)
            {
                NotFound(thesaurusId.ToString());
            }
            return View(GetFormDataOut(form));
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult GetFormJson([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            FormDataOut dataOut = Mapper.Map<FormDataOut>(formDataIn);

            return PartialView("~/Views/Form/DragAndDrop/FormJson.cshtml", dataOut);
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult GetAllCommentsByForm(string formId)
        {
            Form form = formBLL.GetFormById(formId);
            List<string> formItemsOrderIds = form.GetIdsFromObject();
            List<FormCommentDataOut> commentsDataOut = commentBLL.GetComentsDataOut(formId, formItemsOrderIds);

            return PartialView("~/Views/Form/DragAndDrop/FormAllComments.cshtml", commentsDataOut);
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult AddCommentSection(string fieldId)
        {
            ViewBag.ItemRef = fieldId;
           
            return PartialView("~/Views/Form/DragAndDrop/FormCommentSection.cshtml");
        }

        

        [SReportsAutorize]
        [HttpPost]
        public ActionResult AddComment([ModelBinder(typeof(JsonNetModelBinder))] FormCommentDataIn commentDataIn)
        {
            commentDataIn = Ensure.IsNotNull(commentDataIn, nameof(commentDataIn));
            commentDataIn.UserId = userCookieData.Id;
            commentBLL.InsertOrUpdate(commentDataIn);

            return GetAllCommentsByForm(commentDataIn.FormRef);
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult ReplayComment(string commText, int commentId)
        {
            string formRef = commentBLL.ReplayComment(commText, commentId, userCookieData.Id);
            
            return GetAllCommentsByForm(formRef);
        }

        [SReportsAutorize]
        [HttpPost]
        public ActionResult SendCommentStatus(int commentId, CommentState status)
        {
            string formRef = commentBLL.UpdateState(commentId, status);

            return GetAllCommentsByForm(formRef);
        }

        [SReportsAutorize]
        public ActionResult GetDocumentsByThesaurusId(int o4MtId, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, string.Empty);
            ViewBag.TotalAppeareance = formDAL.GetThesaurusAppereanceCount(o4MtId, string.Empty);

            if (thesaurusPageNum != 0)
                return PartialView("FormTreePartial", result);

            return PartialView("FormTree", result);
        }

        [SReportsAutorize]
        public ActionResult ReloadClinicalDomain(string term)
        {
            List<ClinicalDomainDTO> options = Enum.GetNames(typeof(DocumentClinicalDomain))
                .Where(x => x.ToLower().Contains(term.ToLower()))
                .Select(x => new ClinicalDomainDTO()
                {
                    Id = (int)Enum.Parse(typeof(DocumentClinicalDomain), x),
                    Translation = TextLanguage.ResourceManager.GetString(x)
                })
                .ToList();

            return PartialView("~/Views/Form/DragAndDrop/CustomFields/ClinicalDomainValues.cshtml", options.OrderBy(x => x.Translation).ToList());
        }

        [SReportsAutorize]
        public ActionResult FilterThesaurusTree(int o4MtId, string searchTerm, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, searchTerm);
            ViewBag.TotalAppeareance = formDAL.GetThesaurusAppereanceCount(o4MtId, searchTerm);

            if (thesaurusPageNum != 0)
                return PartialView("FormThesaurusTreePartial", result);

            return PartialView("FormThesaurusTree", result);
        }

        [SReportsAutorize]
        public ActionResult GetDocumentProperties(string id)
        {
            DocumentProperties result = this.formDAL.GetDocumentProperties(id);
            return PartialView("DocumentProperties", Mapper.Map<DocumentPropertiesDataOut>(result));
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]
        [ValidateInput(false)]
        [SReportsFormValidate]
        public ActionResult Create([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn, string formId)
        {
            formDataIn = Ensure.IsNotNull(formDataIn, nameof(formDataIn));

            if (formId != null && !formDAL.ExistsForm(formId))
            {
                Log.Warning(SReportsResource.FormNotExists, 404, formId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Form form = Mapper.Map<Form>(formDataIn);
            UserData userData = Mapper.Map<UserData>(userCookieData);

            form.UserId = userCookieData.Id;
            form.OrganizationId = (userCookieData.GetActiveOrganizationData()?.Id).GetValueOrDefault();
            form.Language = userCookieData.ActiveLanguage;

            formBLL.DisableActiveFormsIfNewVersion(form, userCookieData);

            try
            {
                formDAL.InsertOrUpdate(form, userData);
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
        [SReportsAutorize]
        public ActionResult Delete(string formId, DateTime lastUpdate)
        {
            try
            {
                formDAL.Delete(formId, lastUpdate);
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
            formDAL.InsertOrUpdate(form, Mapper.Map<UserData>(userCookieData));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult GenerateNewLanguage(string formid, string language)
        {
            bool success = formBLL.TryGenerateNewLanguage(formid, language, userCookieData);
            
           return success ? new HttpStatusCodeResult(HttpStatusCode.Created) : new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public ActionResult GenerateThesauruses(string formId)
        {
            Form form = formDAL.GetForm(formId);
            UserData userData = Mapper.Map<UserData>(userCookieData);
            ThesaurusGenerator generator = new ThesaurusGenerator();
            generator.GenerateThesauruses(form, Mapper.Map<UserData>(userCookieData));
            formDAL.InsertOrUpdate(form, userData);

            return RedirectToAction("Edit", "Form", new { formId });
        }
        
    }
}