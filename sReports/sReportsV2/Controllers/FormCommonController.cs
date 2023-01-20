using AutoMapper;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DTOs.Form;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController : BaseController
    {
        protected readonly IFormInstanceDAL formInstanceDAL;
        protected readonly IFormDAL formDAL;
        protected readonly IEncounterDAL encounterDAL;
        protected readonly IUserBLL userBLL;
        protected readonly IOrganizationBLL organizationBLL;
        public ICustomEnumBLL customEnumBLL;
        public IFormInstanceBLL formInstanceBLL;
        public IFormBLL formBLL;
        public readonly IEpisodeOfCareDAL episodeOfCareDAL;
        public readonly IPatientDAL patientDAL;

        public FormCommonController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL,IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL)
        {
            this.userBLL = userBLL;
            this.organizationBLL = organizationBLL;
            formInstanceDAL = new FormInstanceDAL();
            formDAL = new FormDAL();
            this.encounterDAL = encounterDAL;
            this.episodeOfCareDAL =episodeOfCareDAL;
            this.formInstanceBLL = formInstanceBLL;
            this.formBLL = formBLL;
            this.customEnumBLL = customEnumBLL;
            this.patientDAL = patientDAL;
        }

        protected void SetDependables(Form form, List<FieldDataOut> allFields)
        {
            form = Ensure.IsNotNull(form, nameof(form));
            List<FieldDataOut> fields = Mapper.Map<List<FieldDataOut>>(form.GetAllFields());
            foreach (FieldSelectableDataOut formFieldDataOut in allFields.OfType<FieldSelectableDataOut>())
            {
                formFieldDataOut.GetDependablesData(fields, formFieldDataOut.Dependables);
            }
        }

        protected FormDataOut GetFormDataOut(Form form)
        {
            ViewBag.States = Enum.GetValues(typeof(FormDefinitionState)).Cast<FormDefinitionState>().ToList();           
            return formBLL.GetFormDataOut(form, userCookieData);
        }

        public string RenderPartialViewToString(string viewName, object model, bool isChapterReadonly, int fieldSetCounter, string fieldSetId)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;
            ViewBag.Chapter = isChapterReadonly;
            ViewBag.FieldSetCounter = fieldSetCounter;
            ViewBag.FieldSetId = fieldSetId;
           


            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        protected void SetViewBagAndMakeResetAndNeSectionHidden()
        {
            ViewBag.ShowResetAndNeSection = false;
        }

        protected void PopulateFormStates(FormFilterDataIn dataIn)
        {
            foreach (var state in (FormDefinitionState[])Enum.GetValues(typeof(FormDefinitionState)))
                dataIn.FormStates.Add(Resources.TextLanguage.ResourceManager.GetString(state.ToString()));
        }

        #region FormInstanceFromRequest
        protected FormInstance GetFormInstanceSet(Form form)
        {
            FormInstance formInstanceFromBase = formInstanceDAL.GetById(Request.Form["formInstanceId"]);
            string notes = Request.Form["notes"] ?? string.Empty;
            string date = Request.Form["date"] ?? string.Empty;
            string formState = Request.Form["formState"];
            FormInstance result = new FormInstance(form)
            {
                UserId = (userCookieData?.Id).GetValueOrDefault(),
                OrganizationId = (userCookieData?.ActiveOrganization).GetValueOrDefault(),
                PatientId = formInstanceFromBase != null ? formInstanceFromBase.PatientId : 0,
                EpisodeOfCareRef = formInstanceFromBase != null ? formInstanceFromBase.EpisodeOfCareRef : 0,
                Notes = notes,
                Date = string.IsNullOrWhiteSpace(date) ? DateTime.Now : DateTime.Parse(date).ToLocalTime(),
                FormState = string.IsNullOrWhiteSpace(formState) ? FormState.Finished : (FormState)Enum.Parse(typeof(FormState), formState)
            };

            result.Id = Request.Form["formInstanceId"];
            result.Referrals = Request.Form["referrals"]?.Split(',')?.ToList() ?? new List<string>();
            if (string.IsNullOrWhiteSpace(Request.Form["LastUpdate"]))
            {
                result.SetLastUpdate();
            } 
            else
            {
                result.LastUpdate = Convert.ToDateTime(Request.Form["LastUpdate"]);
            }

            SetFieldsIntoFormInstance(result, form);

            return result;
        }

        private void SetFieldsIntoFormInstance(FormInstance formInstance, Form form)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));
            form = Ensure.IsNotNull(form, nameof(form));

            formInstance.Fields = new List<FieldValue>();

            List<Field> fields = new List<Field>();
            List<string> keys = new List<string>();
            foreach (FieldSet fieldSet in form.GetAllFieldSets())
            {
                keys = Request.Form.AllKeys.Where(x => x.StartsWith($"{fieldSet.Id}-")).ToList();

                List<string> distinctedList = GetDistinctedList(keys);

                Dictionary<string, int> dictionaryOfPosition = GetDictionaryOfPosition(distinctedList);
                AddCloneOfFieldSet(form.GetAllListOfFieldSets().FirstOrDefault(x => x[0].Id == fieldSet.Id), distinctedList.Count);

                foreach (string key in keys)
                {
                    string instanceId = $"{key.GetFieldSetId()}-{dictionaryOfPosition[key.GetFieldSetCounter()]}-{key.GetFieldId()}-{key.GetFieldCounter()}";
                    Field field = fields.FirstOrDefault(x => x.Id == key.GetFieldId() && x.InstanceId.GetFieldSetCounter() == key.GetFieldSetCounter());
                    if (field == null)
                    {
                        field = form.GetFieldById(key.GetFieldId()).Clone();
                    }
                    field.SetValue(Request.Form[key]);
                    field.InstanceId = instanceId;

                    RemoveIfExist(fields, field);

                    fields.Add(field);
                }
            }

            formInstance.Fields = fields.Select(x => new FieldValue() { Id = x.Id, ThesaurusId = x.ThesaurusId, InstanceId = x.InstanceId, Value = x.Value, ValueLabel = x.GetValueLabelsFromValue(), Type = x.Type }).ToList();

        }

        private void RemoveIfExist(List<Field> fields, Field field)
        {
            var fieldExist = fields.FirstOrDefault(x => x.InstanceId == field.InstanceId);
            if (fieldExist != null)
            {
                fields.Remove(fieldExist);
            }
        }

        private void SetFormInstanceFieldSets(List<List<FieldSet>> listOfFieldSets)
        {
            listOfFieldSets = Ensure.IsNotNull(listOfFieldSets, nameof(listOfFieldSets));

            foreach (List<FieldSet> fieldSets in listOfFieldSets)
            {
                string fieldSetId = fieldSets[0].Id;
                List<string> allKeysForFieldSetId = Request.Form.AllKeys.Where(x => x.StartsWith($"{fieldSetId}-")).ToList();
                List<string> distinctedList = GetDistinctedList(allKeysForFieldSetId);
                Dictionary<string, int> dictionaryOfPosition = GetDictionaryOfPosition(distinctedList);
                AddCloneOfFieldSet(fieldSets, distinctedList.Count);
                SetValues(fieldSets, allKeysForFieldSetId, dictionaryOfPosition);

            }
        }

        public void SetValues(List<FieldSet> fieldSets, List<string> allKeysForFieldSetId, Dictionary<string, int> dictionaryOfPosition)
        {
            allKeysForFieldSetId = Ensure.IsNotNull(allKeysForFieldSetId, nameof(allKeysForFieldSetId));
            fieldSets = Ensure.IsNotNull(fieldSets, nameof(fieldSets));
            dictionaryOfPosition = Ensure.IsNotNull(dictionaryOfPosition, nameof(dictionaryOfPosition));

            foreach (string key in allKeysForFieldSetId)
            {
                string[] ids = key.Split('-');

                Field field = fieldSets[dictionaryOfPosition[ids[1]]].Fields.FirstOrDefault(x => x.Id.Equals(ids[2]));

                field.InstanceId = $"{ids[0]}-{dictionaryOfPosition[ids[1]]}-{ids[2]}-{ids[3]}";

                field.SetValue(Request.Form[key]);
            }
        }

        public void AddCloneOfFieldSet(List<FieldSet> fieldSets, int targetSize)
        {
            fieldSets = Ensure.IsNotNull(fieldSets, nameof(fieldSets));
            if (fieldSets == null)
            {
                throw new ArgumentNullException(nameof(fieldSets));
            }

            //reset value just in case form contains some default value
            foreach (Field field in fieldSets[0].Fields)
            {
                field.Value = null;
            }

            while (true)
            {
                if (fieldSets.Count >= targetSize)
                {
                    break;
                }
                fieldSets.Add(fieldSets[0].Clone());
            }
        }

        private List<string> GetDistinctedList(List<string> allKeysForFieldSetId)
        {
            List<string> listForDistinct = new List<string>();

            foreach (string value in allKeysForFieldSetId)
            {
                listForDistinct.Add(value.Split('-')[1]);
            }

            return listForDistinct.Distinct().ToList();
        }

        private Dictionary<string, int> GetDictionaryOfPosition(List<string> distinctedList)
        {
            Dictionary<string, int> dictionaryOfPosition = new Dictionary<string, int>();

            for (int i = 0; i < distinctedList.Count; i++)
            {
                dictionaryOfPosition.Add(distinctedList[i], i);
            }

            return dictionaryOfPosition;
        }
        #endregion
    }
}