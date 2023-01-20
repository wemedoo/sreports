using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Common.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Field;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using System.Net;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Controllers
{
    public partial class FormController
    {
        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetPredefinedFormElements()
        {
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/DragAndDrop/PredefinedFormElements.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult CreateDragAndDropFormPartial([ModelBinder(typeof(JsonNetModelBinder))]  FormDataIn formDataIn)
        {
            SetViewBagPreviewTypeParameters();
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/DragAndDrop/DragAndDropFormPartial.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult CreateFormTreeNestable([ModelBinder(typeof(JsonNetModelBinder))]  FormDataIn formDataIn)
        {
            return PartialView("~/Views/Form/DragAndDrop/FormTreeNestable.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }
      
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Designer)]
        public ActionResult Create()
        {
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return View("~/Views/Form/DragAndDrop/Create.cshtml");
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Designer)]
        public ActionResult Edit(int thesaurusId, string versionId, string activeTab, string taggedCommentId)
        {
            Form form = this.formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, versionId);
            if (form == null)
            {
                return NotFound(thesaurusId.ToString());
            }
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();
            ViewBag.Consensus = Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(form.Id));
            ViewBag.ActiveTab = activeTab;
            ViewBag.TaggedCommentId = taggedCommentId;
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return View("~/Views/Form/DragAndDrop/Create.cshtml", GetFormDataOut(form));
        }

        [SReportsAuditLog]
        [Authorize]
        [HttpPost]
        public ActionResult GetFormTree([ModelBinder(typeof(JsonNetModelBinder))]  FormDataIn formDataIn)
        {
            SetViewBagPreviewTypeParameters();
            SetViewBagCommentsParameters();
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/DragAndDrop/FormTreeContainer.cshtml", Mapper.Map<FormDataOut>(formDataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFormGeneralInfoForm(FormDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();

            int formInstancesCount = (dataIn != null && dataIn.Id != "formIdPlaceHolder") ? formInstanceDAL.CountByDefinition(dataIn.Id) : -1;
            ViewBag.NotUpdateableField = (formInstancesCount > 0) ? true : false;

            SetViewBagPreviewTypeParameters();

            return PartialView("~/Views/Form/DragAndDrop/FormGeneralInfo.cshtml", Mapper.Map<FormDataOut>(dataIn));
        }

        // This endpoint is NOT in use
        /*[SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFormPreview([ModelBinder(typeof(JsonNetModelBinder))] FormDataIn dataIn)
        {
            return PartialView("~/Views/Form/DragAndDrop/FormPartialPreview.cshtml", Mapper.Map<FormDataOut>(dataIn));
        }*/

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetFormAdministrativeData(string formId)
        {
            Form form = formBLL.GetFormById(formId);

            return PartialView("~/Views/Form/DragAndDrop/CustomFields/FormAdministrativeDataWorkflow.cshtml", GetFormDataOut(form).GetWorkflowHistory());
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetChapterInfoForm(FormChapterDataIn chapter)
        {
            SetViewBagPreviewTypeParameters();
            return PartialView("~/Views/Form/DragAndDrop/ChapterInfoForm.cshtml", Mapper.Map<FormChapterDataOut>(chapter));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetPageInfoForm(FormPageDataIn chapter, string parentId)
        {
            ViewBag.ParentId = parentId;
            SetViewBagPreviewTypeParameters();
            return PartialView("~/Views/Form/DragAndDrop/PageInfoForm.cshtml", Mapper.Map<FormPageDataOut>(chapter));
        }


        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldSetInfoForm(FormFieldSetDataIn chapter, string parentId)
        {
            ViewBag.ParentId = parentId;
            SetViewBagPreviewTypeParameters();
            return PartialView("~/Views/Form/DragAndDrop/FieldSetInfoForm.cshtml", Mapper.Map<FormFieldSetDataOut>(chapter));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldInfoForm([ModelBinder(typeof(JsonNetModelBinder))]FieldDataIn fieldDataIn, string parentId)
        {
            ViewBag.ParentId = parentId;
            SetViewBagPreviewTypeParameters();
            return PartialView("~/Views/Form/DragAndDrop/FieldInfoCommonForm.cshtml", Mapper.Map<FieldDataOut>(fieldDataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpPost]
        public ActionResult GetFieldInfoCustomForm([ModelBinder(typeof(JsonNetModelBinder))] FieldDataIn fieldDataIn)
        {
            fieldDataIn = Ensure.IsNotNull(fieldDataIn, nameof(fieldDataIn));
            SetViewBagPreviewTypeParameters();
            var dataOut = Mapper.Map<FieldDataOut>(fieldDataIn);
            switch (fieldDataIn.Type)
            {
                case FieldTypes.Calculative:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CalculativeFieldForm.cshtml", dataOut);
                case FieldTypes.Checkbox:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CheckboxFieldForm.cshtml", dataOut);
                case FieldTypes.CustomButton:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/CustomFieldButtonForm.cshtml", dataOut);
                case FieldTypes.Date:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/DateFieldForm.cshtml", dataOut);
                case FieldTypes.Datetime:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/DatetimeFieldForm.cshtml", dataOut);
                case FieldTypes.Email:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/EmailFieldForm.cshtml", dataOut);
                case FieldTypes.File:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/FileFieldForm.cshtml", dataOut);
                case FieldTypes.LongText:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/LongTextFieldForm.cshtml", dataOut);
                case FieldTypes.Number:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/NumberFieldForm.cshtml", dataOut);
                case FieldTypes.Radio:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/RadioFieldForm.cshtml", dataOut);
                case FieldTypes.Regex:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/RegexFieldForm.cshtml", dataOut);
                case FieldTypes.Select:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/SelectFieldForm.cshtml", dataOut);
                case FieldTypes.Text:
                    return PartialView("~/Views/Form/DragAndDrop/CustomFields/TextFieldForm.cshtml", dataOut);
                default: return PartialView("~/Views/Form/DragAndDrop/CustomFields/TextFieldForm.cshtml", dataOut);
            }
            
        }


        public ActionResult GetFieldValueInfoForm([ModelBinder(typeof(JsonNetModelBinder))]FormFieldValueDataIn fieldValueDataIn, string parentId)
        {
            ViewBag.ParentId = parentId;
            SetViewBagPreviewTypeParameters();
            return PartialView("~/Views/Form/DragAndDrop/FieldValueInfoForm.cshtml", Mapper.Map<FormFieldValueDataOut>(fieldValueDataIn));
        }

        [HttpPost]
        public ActionResult GetCalculativeTree(CalculativeTreeDataDTO dataIn)
        {
            SetViewBagPreviewTypeParameters();
            return PartialView(dataIn.Data);
        }

        private void SetViewBagPreviewTypeParameters()
        {
            SetReadOnlyAndDisabledViewBag(!ViewBag.UserCookieData.UserHasPermission(PermissionNames.CreateUpdate, ModuleNames.Designer));
        }
    }
}