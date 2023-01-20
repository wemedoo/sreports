using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs;
using sReportsV2.DTOs.DocumentProperties.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.Form.DTO;
using sReportsV2.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.BusinessLayer.Interfaces;
using Generator;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using System.Web;
using sReportsV2.Common.Constants;

namespace sReportsV2.Controllers
{
    [SReportsAuthorize]
    public partial class FormController : FormCommonController
    {

        public ILogger Logger;
        private readonly ICommentBLL commentBLL;
        private readonly IConsensusDAL consensusDAL;
        public FormController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IConsensusDAL consensusDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL, ICommentBLL commentBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL ,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
            this.customEnumBLL = customEnumBLL;
            this.commentBLL = commentBLL;
            this.consensusDAL = consensusDAL;
        }


        [SReportsAuthorize(Module = ModuleNames.Designer, Permission = PermissionNames.View)]
        [SReportsAuditLog]
        public ActionResult GetAll(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize(Module = ModuleNames.Designer, Permission = PermissionNames.View)]
        public ActionResult ReloadTable(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PopulateFormStates(dataIn);

            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            return PartialView("FormsTable", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAuthorize(Module = ModuleNames.Engine, Permission = PermissionNames.View)]
        public ActionResult ReloadByFormThesaurusTable(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PopulateFormStates(dataIn);
            //dataIn.State = Domain.Enums.FormDefinitionState.ReadyForDataCapture;
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            return PartialView("~/Views/FormInstance/FormsTable.cshtml", this.formBLL.ReloadData(dataIn, userCookieData));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult Get(int thesaurusId)
        {
            Form form = this.formBLL.GetFormByThesaurusAndLanguage(thesaurusId, userCookieData.ActiveLanguage);
            if (form == null)
            {
                return NotFound(thesaurusId.ToString());
            }
            return View(GetFormDataOut(form));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.ChangeState, Module = ModuleNames.Designer)]
        [HttpPut]
        public ActionResult UpdateFormState(UpdateFormStateDataIn updateFormStateDataIn)
        {
            var result = formBLL.UpdateFormState(updateFormStateDataIn, userCookieData);
            return Json(result);
        }

        [SReportsAuthorize(Permission = PermissionNames.ShowJson, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetFormJson([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn formDataIn)
        {
            FormDataOut dataOut = Mapper.Map<FormDataOut>(formDataIn);

            return PartialView("~/Views/Form/DragAndDrop/FormJson.cshtml", dataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.ViewComments, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetAllCommentsByForm(string formId, string taggedCommentId)
        {
            Form form = formBLL.GetFormById(formId);
            List<string> formItemsOrderIds = form.GetIdsFromObject();
            List<FormCommentDataOut> commentsDataOut = commentBLL.GetComentsDataOut(formId, formItemsOrderIds);
            SetViewBagCommentsParameters(taggedCommentId);
            return PartialView("~/Views/Form/DragAndDrop/FormAllComments.cshtml", commentsDataOut);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddComment, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult AddCommentSection(string fieldId)
        {
            ViewBag.ItemRef = fieldId;
           
            return PartialView("~/Views/Form/DragAndDrop/FormCommentSection.cshtml");
        }



        [SReportsAuthorize(Permission = PermissionNames.AddComment, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult AddComment([ModelBinder(typeof(JsonNetModelBinder))] FormCommentDataIn commentDataIn)
        {
            commentDataIn = Ensure.IsNotNull(commentDataIn, nameof(commentDataIn));
            commentDataIn.UserId = userCookieData.Id;
            commentBLL.InsertOrUpdate(commentDataIn);

            return GetAllCommentsByForm(commentDataIn.FormRef, null);
        }

        [SReportsAuthorize(Permission = PermissionNames.AddComment, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult ReplayComment(string commText, int commentId, List<int> taggedUsers)
        {
            string formRef = commentBLL.ReplayComment(commText, commentId, userCookieData.Id, taggedUsers);
            
            return GetAllCommentsByForm(formRef, null);
        }

        [SReportsAuthorize(Permission = PermissionNames.ChangeCommentStatus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult SendCommentStatus(int commentId, CommentState status)
        {
            string formRef = commentBLL.UpdateState(commentId, status);

            return GetAllCommentsByForm(formRef, null);
        }

        [SReportsAuthorize]
        public ActionResult GetDocumentsByThesaurusId(int o4MtId, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, string.Empty);
            ViewBag.TotalAppeareance = formDAL.GetThesaurusAppereanceCount(o4MtId, string.Empty);

            if (thesaurusPageNum != 0)
                return PartialView("FormTreePartial", result);

            return PartialView("FormTree", result);
        }

        [SReportsAuthorize]
        public ActionResult ReloadClinicalDomain(string term)
        {
            List<ClinicalDomainDTO> options = Enum.GetNames(typeof(DocumentClinicalDomain))
                .Where(x => x.ToLower().Contains(term.ToLower()))
                .Select(x => new ClinicalDomainDTO()
                {
                    Id = (int)Enum.Parse(typeof(DocumentClinicalDomain), x),
                    Translation = TextLanguage.ResourceManager.GetString(x)
                })
                .OrderBy(enm => enm.ToString())
                .ToList();

            return PartialView("~/Views/Form/DragAndDrop/CustomFields/ClinicalDomainValues.cshtml", options.OrderBy(x => x.Translation).ToList());
        }

        [SReportsAuthorize]
        public ActionResult FilterThesaurusTree(int o4MtId, string searchTerm, int thesaurusPageNum)
        {
            TreeDataOut result = formBLL.GetTreeDataOut(o4MtId, thesaurusPageNum, searchTerm, userCookieData);
            ViewBag.TotalAppeareance = formDAL.GetThesaurusAppereanceCount(o4MtId, searchTerm, userCookieData.ActiveOrganization);

            if (thesaurusPageNum != 0)
                return PartialView("FormThesaurusTreePartial", result);

            return PartialView("FormThesaurusTree", result);
        }

        [SReportsAuthorize]
        public ActionResult GetDocumentProperties(string id)
        {
            DocumentProperties result = this.formDAL.GetDocumentProperties(id);
            return PartialView("DocumentProperties", Mapper.Map<DocumentPropertiesDataOut>(result));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Designer)]
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
            formDAL.InsertOrUpdate(form, userData);

            JsonResult jsonResult = new JsonResult()
            {
                Data = new
                {
                    id = form.Id,
                    versionId = form.Version.Id,
                    thesaurusId = form.ThesaurusId,
                    lastUpdate = HttpUtility.UrlEncode(form.LastUpdate.Value.ToString("o"))
                }
            };
            return jsonResult;
        }

        [SReportsAuditLog]
        [HttpDelete]
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Designer)]
        public ActionResult Delete(string formId, DateTime lastUpdate)
        {
            formDAL.Delete(formId, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }


        public ActionResult ExportCTCAEForm()
        {
            FormGenerator generator = new FormGenerator(Mapper.Map<UserData>(userCookieData));
            Form form = generator.GetFormFromCsv("CTCAE ");
            formDAL.InsertOrUpdate(form, Mapper.Map<UserData>(userCookieData));

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Designer)]
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult GenerateNewLanguage(string formid, string language)
        {
            bool success = formBLL.TryGenerateNewLanguage(formid, language, userCookieData);
            if (success)
                return new JsonResult { Data = TextLanguage.GenerateFormTranslationMessage, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            else
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
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

        [SReportsAuthorize]
        public ActionResult RetrieveUser(string searchWord, string commentId)
        {
            List<UserData> userData = userBLL.ProposeUserBySearchWord(searchWord);
            ViewBag.CommentId = commentId;
            return PartialView("~/Views/Form/DragAndDrop/CustomFields/UserValues.cshtml", userData);
        }

        private void SetViewBagCommentsParameters(string taggedCommentId = "")
        {
            ViewBag.TaggedCommentId = taggedCommentId;
            ViewBag.CanAddComment = ViewBag.UserCookieData.UserHasPermission(PermissionNames.AddComment, ModuleNames.Designer);
            ViewBag.CanChangeCommentStatus = ViewBag.UserCookieData.UserHasPermission(PermissionNames.ChangeCommentStatus, ModuleNames.Designer);
        }
    }
}