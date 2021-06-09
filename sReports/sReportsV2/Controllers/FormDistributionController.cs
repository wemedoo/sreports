using AutoMapper;
using Chapters;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormDistribution.DataIn;
using sReportsV2.DTOs.FormDistribution.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Common;
using DocumentGenerator;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;

namespace sReportsV2.Controllers
{
    public class FormDistributionController : FormCommonController
    {
        private IFormDistributionService formDistributionService;
        public FormDistributionController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL, IEncounterDAL encounterDAL) : base(patientDAL, episodeOfCareDAL,encounterDAL,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
            formDistributionService = new FormDistributionService();
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult GetAll(FormFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(FormDistributionFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            PaginationDataOut<FormDistributionTableDataOut, FormDistributionFilterDataIn> result = new PaginationDataOut<FormDistributionTableDataOut, FormDistributionFilterDataIn>()
            {
                Count = formDistributionService.GetAllCount(),
                Data = Mapper.Map<List<FormDistributionTableDataOut>>(formDistributionService.GetAll(dataIn.Page, dataIn.PageSize)),
                DataIn = dataIn
            };
            return PartialView("ReloadTable", result);
        }

        [SReportsAutorize]
        public ActionResult ReloadForms(FormFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.State = FormDefinitionState.ReadyForDataCapture;

            FormFilterData filterData = GetFormFilterData(dataIn);

            PaginationDataOut<FormDataOut, FormFilterDataIn> result = new PaginationDataOut<FormDataOut, FormFilterDataIn>()
            {
                Count = (int)this.formDAL.GetAllFormsCount(filterData),
                Data = Mapper.Map<List<FormDataOut>>(this.formDAL.GetAll(filterData)),
                DataIn = dataIn
            };

            ReloadFormDataOut(result.Data);

            return PartialView(result);
        }

        public void ReloadFormDataOut(List<FormDataOut> forms) 
        {
            var formDistributions = formDistributionService.GetAllVersionAndThesaurus();

            foreach (FormDataOut form in forms)
            {
                    form.IsParameterize = formDistributions.Any(x => x.ThesaurusId == form.ThesaurusId && x.VersionId != null && x.VersionId == form.Version.Id);
            }
        }

        public ActionResult Edit(string formDistributionId)
        {
            var fd = formDistributionService.GetById(formDistributionId);
            var result = Mapper.Map<FormDistributionDataOut>(fd);

            return View(result);
        }
        public ActionResult GetByThesaurusId(int thesaurusId, string versionId)
        {
            FormDistributionDataOut dataOut = null;
            FormDistribution formDistribution = formDistributionService.GetByThesaurusIdAndVersion(thesaurusId, versionId);
            Form form = formDAL.GetFormByThesaurusAndVersion(thesaurusId, versionId);

            if (formDistribution != null)
            {
                dataOut = Mapper.Map<FormDistributionDataOut>(formDistribution);
            }
            else
            {
                if (form != null)
                {
                    formDistribution = GetFromForm(form);
                    dataOut = Mapper.Map<FormDistributionDataOut>(formDistribution);
                }
            }

            List<Field> fields = form.GetAllFields();
            foreach (var field in dataOut.Fields) 
            {
                foreach(var rel in field.RelatedVariables) 
                {
                    rel.Label = fields.FirstOrDefault(x => x.Id == rel.Id).Label;
                }
            }

            ViewBag.Form = GetFormDataOut(form);

            return View("Edit", dataOut);
        }

        private FormDistribution GetFromForm(Form form)
        {
            return new FormDistribution() {
                EntryDatetime = form.EntryDatetime,
                ThesaurusId = form.ThesaurusId,
                Title = form.Title,
                Fields = GetDistributionFields(form.GetAllFields().Where(x => x is FieldCheckbox || x is FieldRadio || x is FieldSelect || x is FieldNumeric).ToList()),
                VersionId = form.Version.Id
            };
        }

        private List<FormFieldDistribution> GetDistributionFields(List<Field> fields)
        {
            List<FormFieldDistribution> result = new List<FormFieldDistribution>();
            foreach(Field field in fields)
            {
                FormFieldDistribution fieldDistribution = new FormFieldDistribution()
                {
                    Id = field.Id,
                    Label = field.Label,
                    RelatedVariables = new List<Domain.Entities.Distribution.RelatedVariable>(),
                    ThesaurusId = field.ThesaurusId,
                    Type = field.Type,
                    ValuesAll = new List<FormFieldDistributionSingleParameter>()
                 {
                     new FormFieldDistributionSingleParameter()
                     {
                         NormalDistributionParameters = new FormFieldNormalDistributionParameters(),
                         Values = field is FieldSelectable ? (field as FieldSelectable).Values.Select(x => new FormFieldValueDistribution()
                         {
                             Label = x.Label,
                             ThesaurusId = x.ThesaurusId,
                             Value = x.Value
                         }).ToList() : null
                     }
                 }
                };
                result.Add(fieldDistribution);
            }

            return result;
        }

        [HttpPost]
        public ActionResult SetParameters(FormDistributionDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormDistribution formDistribution;
            if (!string.IsNullOrEmpty(dataIn.FormDistributionId))
            {
                formDistribution = formDistributionService.GetById(dataIn.FormDistributionId);
            }
            else
            {
                Form form = formDAL.GetFormByThesaurusAndVersion(dataIn.ThesaurusId, dataIn.VersionId);
                formDistribution = GetFromForm(form);
            }

            formDistribution.Fields = Mapper.Map<List<FormFieldDistribution>>(dataIn.Fields);
           
            formDistributionService.InsertOrUpdate(formDistribution);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
        
        public ActionResult GenerateDocuments(string formId, int numOfDocuments)
        {
            Form form = formDAL.GetForm(formId);
            if (form == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            FormDistribution formDistribution = formDistributionService.GetByThesaurusIdAndVersion(form.ThesaurusId, form.Version.Id);
            if (formDistribution == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            UserData userData = Mapper.Map<UserData>(userCookieData);


            List<FormInstance> generated = FormInstanceGenerator.Generate(form, formDistribution, numOfDocuments);
            foreach (FormInstance formInstance in generated)
            {
                SetFormInstanceAdditionalData(form, formInstance, userData);
            }

            InsertListOfFormInstances(generated);

            if(formDistribution.ThesaurusId == 14573)
            {
                GenerateDailyFormInstances(generated, userData);
            }
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPost]
        public ActionResult RenderInputsForDependentVariable(DependentVariableRelatedVariables dataIn)
        {
            Form form = formDAL.GetFormByThesaurus(dataIn.ThesaurusId);
            List<Field> formFields = form.GetAllFields();
            List<FormFieldDistributionSingleParameterDataOut> variables = new List<FormFieldDistributionSingleParameterDataOut>();

            Field targetField = formFields.FirstOrDefault(x => x.Id.Equals(dataIn.TargetVariable));
            Field field1 = null;
            Field field2 = null;

            if (dataIn.RelatedVariables.Count > 0)
            {
                field1 = formFields.FirstOrDefault(x => x.Id.Equals(dataIn.RelatedVariables[0].Id));
                if (dataIn.RelatedVariables.Count > 1)
                {
                    field2 = formFields.FirstOrDefault(x => x.Id.Equals(dataIn.RelatedVariables[1].Id));
                }


                if (field1 is FieldNumeric)
                {
                    foreach(string option in dataIn.RelatedVariables[0].GetOptions())
                    {
                        SetField(targetField, field2, GetSingleValue(field1, option, dataIn.RelatedVariables[0].GetLabelForOption(option)), field2 != null ? dataIn.RelatedVariables[1] : null, variables);
                    }
                }
                else
                {
                    foreach (FormFieldValue value in (field1 as FieldSelectable).Values)
                    {
                        SetField(targetField, field2, GetSingleValue(field1, value.GetValueToStore(field1.Type), value.Label), field2 != null ?dataIn.RelatedVariables[1]: null, variables);
                    }
                }
            }

            FormFieldDistributionDataOut dataOut = new FormFieldDistributionDataOut()
            {
                Type = targetField.Type,
                ValuesCombination = variables,
                Label = targetField.Label,
                Id = targetField.Id,
                ThesaurusId = targetField.ThesaurusId,
                RelatedVariables = dataIn.RelatedVariables
            };

            return PartialView("RenderInputs",dataOut);
        }

        private void SetField(Field targetField, Field field2, SingleDependOnValueDataOut singleValue1, DTOs.FormDistribution.DataIn.RelatedVariable relatedVariable, List<FormFieldDistributionSingleParameterDataOut> variables)
        {
            if (field2 != null)
            {
                if (field2.Type == FieldTypes.Number)
                {
                    foreach (string option in relatedVariable.GetOptions())
                    {
                        List<SingleDependOnValueDataOut> dependOn = new List<SingleDependOnValueDataOut>
                        {
                            singleValue1,
                            GetSingleValue(field2, option, relatedVariable.GetLabelForOption(option))
                        };
                        variables.Add(GetFormFieldDistributionSingleParameterDataOut(dependOn, targetField));
                    }
                }
                else
                {
                    foreach (FormFieldValue value2 in (field2 as FieldSelectable).Values)
                    {
                        List<SingleDependOnValueDataOut> dependOn = new List<SingleDependOnValueDataOut>
                        {
                            singleValue1,
                            GetSingleValue(field2,  value2.GetValueToStore(field2.Type), value2.Label)
                        };
                        variables.Add(GetFormFieldDistributionSingleParameterDataOut(dependOn, targetField));
                    }
                }
            }
            else
            {
                List<SingleDependOnValueDataOut> dependOn = new List<SingleDependOnValueDataOut>
                                {
                                    singleValue1
                                };
                variables.Add(GetFormFieldDistributionSingleParameterDataOut(dependOn, targetField));

            }
        }

        private SingleDependOnValueDataOut GetSingleValue(Field field, string value, string label)
        {
            return new SingleDependOnValueDataOut()
            {
                Id = field.Id,
                FieldLabel = field.Label,
                Type = field.Type,
                Value = value,
                ValueLabel = label
            };
        }

        private FormFieldDistributionSingleParameterDataOut GetFormFieldDistributionSingleParameterDataOut(List<SingleDependOnValueDataOut> dependOn, Field targetField)
        {
            FormFieldDistributionSingleParameterDataOut result = new FormFieldDistributionSingleParameterDataOut()
            {
                DependOn = dependOn,
                NormalDistributionParameters = new FormFieldNormalDistributionParametersDataOut()
            };

            if(targetField is FieldSelectable) 
            {
                result.Values = (targetField as FieldSelectable).Values.Select(x => new FormFieldValueDistributionDataOut()
                {
                    Label = x.Label,
                    ThesaurusId = x.ThesaurusId,
                    Value = x.Value
                }).ToList();
            }

            return result;
        }

        private int ParseAndInsertPatient(FormChapter chapter)
        {
            //TO DO
            //PatientParser patientParser = new PatientParser(identifierTypeService.GetAll().Where(x => x.Type == IdentifierKind.PatientIdentifierType.ToString()).ToList());
            PatientParser patientParser = new PatientParser(new List<Domain.Sql.Entities.Common.CustomEnum>());
            Patient patient = patientParser.ParsePatientChapter(chapter);

            return InsertPatient(patient);
        }

        private void SetFormDistributionValues(FormDistribution formDistribution, FormDistributionDataIn dataIn)
        {
            formDistribution.LastUpdate = dataIn.LastUpdate;
            foreach(FormFieldDistribution field in formDistribution.Fields)
            {
                FormFieldDistributionDataIn fieldDataIn = dataIn.Fields.FirstOrDefault(x => x.Id.Equals(field.Id));
                if(fieldDataIn != null)
                {
                    SetFormFieldDistributionValues(fieldDataIn, field);
                }
            }
        }

        private void SetFormFieldDistributionValues(FormFieldDistributionDataIn fieldDataIn, FormFieldDistribution field)
        {
            if (fieldDataIn.Values != null)
            {
                foreach (FormFieldValueDistribution value in field.Values)
                {
                    /*FormFieldValueDistributionDataIn valueDataIn = fieldDataIn.Values.FirstOrDefault(x => x.ThesaurusId.Equals(value.ThesaurusId));
                    value.SuccessProbability = valueDataIn?.SuccessProbability;*/
                }
            }
        }

        public string ShowField(FormFieldDistributionDataOut f)
        {
            f = Ensure.IsNotNull(f, nameof(f));

            switch (f.Type)
            {
                case FieldTypes.Number:
                case FieldTypes.Digits:
                    return RenderPartialViewToString("~/Views/FormDistribution/NormalDistributionParameters.cshtml", f);
                case FieldTypes.Checkbox:
                    return RenderPartialViewToString("~/Views/FormDistribution/BinominalDistributionParameters.cshtml", f);
                case FieldTypes.Radio:
                case FieldTypes.Select:
                    return RenderPartialViewToString("~/Views/FormDistribution/MultinominalDistributionParameters.cshtml", f);
                default:
                    return string.Empty;
            }
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private FormFilterData GetFormFilterData(FormFilterDataIn formDataIn)
        {
            FormFilterData result = Mapper.Map<FormFilterData>(formDataIn);
            result.OrganizationId = (userCookieData.GetActiveOrganizationData()?.Id).GetValueOrDefault();
            result.ActiveLanguage = userCookieData.ActiveLanguage;
            return result;
        }

        /*HACKATON*/

        private void GenerateDailyFormInstances(List<FormInstance> generated, UserData user)
        {
            FormDistribution dailyFormDistribution = formDistributionService.GetByThesaurusId(14911);
            Form formDaily = formDAL.GetFormByThesaurus(dailyFormDistribution.ThesaurusId);
            List<FormInstance> dailyGenerated = FormInstanceGenerator.GenerateDailyForms(generated, formDaily, dailyFormDistribution);

            foreach (FormInstance formInstance in dailyGenerated)
            {
                SetFormInstanceAdditionalData(formDaily, formInstance, user);
            }

            InsertListOfFormInstances(dailyGenerated);

        }

        private void SetFormInstanceAdditionalData(Form form, FormInstance formInstance, UserData user)
        {
            if (!form.DisablePatientData)
            {
                int patientId = ParseAndInsertPatient(form.Chapters.FirstOrDefault(x => x.ThesaurusId.Equals("9356")));
                int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Simulator", DateTime.Now, user);
                formInstance.PatientId = patientId;
                formInstance.EpisodeOfCareRef = eocId;
                formInstance.EncounterRef = InsertEncounter(eocId);
            }

            formInstance.UserId = userCookieData.Id;
            formInstance.OrganizationId = userCookieData.ActiveOrganization;
        }
        private void InsertListOfFormInstances(List<FormInstance> formInstances)
        {
            int skip = 0;
            int take = 50;
            while (skip < formInstances.Count)
            {
                formInstanceDAL.InsertMany(formInstances.Skip(skip).Take(take).ToList());
                skip += take;
            }
        }
    }
}