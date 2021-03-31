
using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Models.Form;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class FormCommonController : BaseController
    {
        protected readonly IFormInstanceService formInstanceService;
        protected readonly IFormService formService;
        protected readonly IEncounterService encounterService;
        protected readonly IPatientService patientService;
        protected readonly IEpisodeOfCareService episodeOfCareService;


        public FormCommonController()
        {
            formInstanceService = new FormInstanceService();
            formService = new FormService();
            encounterService = new EncounterService();
            patientService = new PatientService();
            episodeOfCareService = new EpisodeOfCareService();
        }

        protected void SetDependables(Form form, List<FieldDataOut> allFields)
        {
            List<FieldDataOut> fields = Mapper.Map<List<FieldDataOut>>(form.GetAllFields());
            foreach (FieldSelectableDataOut formFieldDataOut in allFields.OfType<FieldSelectableDataOut>())
            {
                formFieldDataOut.GetDependablesData(fields, formFieldDataOut.Dependables);
            }
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

        protected FormInstance GetFormInstanceSet(Form form)
        {
            FormInstance formInstanceFromBase = formInstanceService.GetById(Request.Form["formInstanceId"]);
            string notes = Request.Form["notes"] ?? string.Empty;
            string date = Request.Form["date"] ?? string.Empty;
            string formState = Request.Form["formState"];
            FormInstance result = new FormInstance(form)
            {
                UserRef = userCookieData?.Id,
                OrganizationRef = userCookieData?.ActiveOrganization,
                PatientRef = formInstanceFromBase?.PatientRef,
                EpisodeOfCareRef = formInstanceFromBase?.EpisodeOfCareRef,
                Notes = notes,
                Date = string.IsNullOrWhiteSpace(date) ? DateTime.Now : DateTime.Parse(date).ToLocalTime(),
                FormState = string.IsNullOrWhiteSpace(formState) ? FormState.Finished : (FormState)Enum.Parse(typeof(FormState), formState)
            };

            result.Id = Request.Form["formInstanceId"];
            result.Referrals = Request.Form["referrals"]?.Split(',')?.ToList() ?? new List<string>();
            result.LastUpdate = !string.IsNullOrWhiteSpace(Request.Form["LastUpdate"]) ? Convert.ToDateTime(Request.Form["LastUpdate"]) : DateTime.Now;

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
                    string[] values = key.Split('-');
                    string instanceId = $"{values[0]}-{dictionaryOfPosition[values[1]]}-{values[2]}-{values[3]}";
                    Field field = fields.FirstOrDefault(x => x.Id == key.Split('-')[2] && x.InstanceId == instanceId);
                    if (field == null)
                    {
                        field = form.GetFieldById(key.Split('-')[2]).Clone();
                    }
                    field.SetValue(Request.Form[key]);
                    field.InstanceId = instanceId;

                    RemoveIfExist(fields, field);

                    fields.Add(field);
                }
            }

            formInstance.Fields = fields.Select(x => new FieldValue() { Id = x.Id, ThesaurusId = x.ThesaurusId, InstanceId = x.InstanceId, Value = x.Value, Type = x.Type }).ToList();

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

        public void SetValues(List<FieldSet> fieldSets ,List<string> allKeysForFieldSetId, Dictionary<string, int> dictionaryOfPosition) 
        {
            if (allKeysForFieldSetId == null)
            {
                throw new ArgumentNullException(nameof(allKeysForFieldSetId));
            }

            if (fieldSets == null)
            {
                throw new ArgumentNullException(nameof(fieldSets));
            }

            if (dictionaryOfPosition == null)
            {
                throw new ArgumentNullException(nameof(dictionaryOfPosition));
            }


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

            for (int i = 0; i < distinctedList.Count ; i++)
            {
                dictionaryOfPosition.Add(distinctedList[i], i);
            }

            return dictionaryOfPosition;
        }

        protected Form GetForm(FormInstance formInstance)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            Form form = null;
            if (!formInstance.Language.Equals(userCookieData.ActiveLanguage))
            {
                form = this.formService.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formInstance.ThesaurusId, userCookieData.GetActiveOrganizationData()?.Id, userCookieData.ActiveLanguage, formInstance.Version.Id);  
            }
            if(form == null) 
            {
                form = this.formService.GetForm(formInstance.FormDefinitionId);
            }


            return new Form(formInstance,form);
        }

        public FormDataOut GetFormDataOut(FormInstance formInstance, List<FormInstance> referrals)
        {
            Form form = GetForm(formInstance);
            FormDataOut data = Mapper.Map<FormDataOut>(form);
            data.ReferrableFields = GetReferrableFields(form, GetFormsFromReferrals(referrals));
            data.SetDependables();

            return data;
        }

        protected string InsertPatient(PatientEntity patient)
        {
            string patientId = string.Empty;
            if (patient != null)
            {
                patientId = patient != null && patient.Identifiers != null && patient.Identifiers.Count > 0 ?
                    patientService.GetByIdentifier(patient.Identifiers[0])?.Id?.ToString()
                    :
                    string.Empty;

                if (string.IsNullOrEmpty(patientId))
                {
                    patientId = patientService.Insert(patient);
                }
            }
            return patientId;
        }

        protected string InsertEpisodeOfCare(string patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate)
        {
            EpisodeOfCareEntity eoc;
            if (episodeOfCare != null)
            {
                eoc = Mapper.Map<EpisodeOfCareEntity>(episodeOfCare);
                eoc.Period=new Period() {Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.DiagnosisRole = "12227";
                eoc.OrganizationRef = userCookieData.ActiveOrganization;
            }
            else
            {
                eoc = new EpisodeOfCareEntity()
                {
                    Description = $"Generated from {source}",
                    DiagnosisRole = "12227",
                    OrganizationRef = userCookieData.ActiveOrganization,
                    PatientId = patientId,
                    Status = EOCStatus.Active,
                    Period = new Period() { Start = startDate }
                };
            }

            return episodeOfCareService.InsertOrUpdate(eoc);

        }

        protected string InsertEncounter(string episodeOfCareId)
        {
            EncounterEntity encounterEntity = new EncounterEntity()
            {
                Class = "12246",
                Period = new Period()
                {
                    Start = DateTime.Now,
                    End = DateTime.Now
                },
                Status = "12218",
                EpisodeOfCareId = episodeOfCareId,
                Type = "12208",
                ServiceType = "11087"
            };
            return encounterService.Insert(encounterEntity);
        }

        protected List<ReferralInfoDTO> GetReferrableFields(Form form, List<Form> referrals)
        {
            
            form = Ensure.IsNotNull(form, nameof(form));
            List<ReferralInfoDTO> result = new List<ReferralInfoDTO>();

            if (referrals != null)
            {
                result =  Mapper.Map<List<ReferralInfoDTO>>(form.GetValuesFromReferrals(referrals));
            }

            return result;
        }

        protected List<Form> GetFormsFromReferrals(List<FormInstance> referrals)
        {
            List<Form> forms = new List<Form>();
            foreach (FormInstance referral in referrals)
            {
                Form form = formService.GetForm(referral.FormDefinitionId);
                form.SetFields(referral.Fields);
                forms.Add(form);
            }
            return forms;
        }
    }
}