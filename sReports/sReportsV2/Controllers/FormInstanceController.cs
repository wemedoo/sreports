using AutoMapper;
using RestSharp;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.DTOs;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.Constants;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.Models.Omnia;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using System.Web.Services;
using sReportsV2.Common.Helpers;
using System.Web;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DAL.Sql.Interfaces;
using System.Text.Json;
using System.Data;
using System.Data;
using System.Data.Entity.Core;

namespace sReportsV2.Controllers
{
    //[Authorize]
    public class FormInstanceController : FormCommonController
    {

        private static ObjectCache cache = MemoryCache.Default;
        
        #region Before Hackaton
        public FormInstanceController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        //[Authorize]
        public ActionResult GetAllByFormThesaurus(FormInstanceFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            if (!this.formDAL.ExistsFormByThesaurus(dataIn.ThesaurusId))
            {
                Log.Warning(SReportsResource.FormForThesaurusIdNotExists, 404, dataIn.ThesaurusId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Form form = this.formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(dataIn.ThesaurusId, userCookieData.ActiveOrganization, dataIn.Language ?? userCookieData.ActiveLanguage, dataIn.VersionId);
            if (form == null)
            {
                return RedirectToAction("GetAllFormDefinitions");
            }

            //filter data
            dataIn.Title = form.Title;
            dataIn.FormId = form.Id;
            ViewBag.FilterFormInstanceDataIn = dataIn;
            ViewBag.Language = dataIn.Language;
            if (dataIn.IsSimplifiedLayout)
            {
                ViewBag.Language = dataIn.Language;
            }
            return View("GetAllByFormThesaurus", masterName: dataIn.IsSimplifiedLayout ? "_Crf_Layout" : "_Layout");
        }

        public ActionResult GetAllFormInstance(FormInstanceCovidFilterDataIn filter)
        {
            List<FormInstance> resultData = this.formInstanceDAL.GetAllByCovidFilter(Mapper.Map<FormInstanceCovidFilter>(filter));

            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = serializer.Serialize(resultData),
                ContentType = "application/json"
            };

            return result;
        }

        public async Task<ActionResult> GetTest()
        {

            var serializer = new JavaScriptSerializer();

            // For simplicity just use Int32's max value.
            // You could always read the value from the config section mentioned above.
            serializer.MaxJsonLength = Int32.MaxValue;
            return new ContentResult
            {
                Content = serializer.Serialize(await formInstanceDAL.GetAllFieldsByCovidFilter().ConfigureAwait(false)),
                ContentType = "application/json"
            };
            //return Json(await formInstanceService.GetAllFieldsByCovidFilter().ConfigureAwait(false), JsonRequestBehavior.AllowGet, JsonRequestBehavior);
        }

        //[Authorize]
        [SReportsAuthorize]
        public ActionResult ReloadByFormThesaurusTable(FormInstanceFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            if (!this.formDAL.ExistsFormByThesaurus(dataIn.ThesaurusId))
            {
                Log.Warning(SReportsResource.FormForThesaurusIdNotExists, 404, dataIn.ThesaurusId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.FilterFormInstanceDataIn = dataIn;
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());
            ViewBag.FormInstanceTitle = dataIn.Title;

            return PartialView("FormInstancesByFormThesaurusTable", formInstanceBLL.ReloadData(dataIn));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult GetAllFormDefinitions(FormFilterDataIn dataIn)
        {
            ViewBag.DocumentPropertiesEnums = Mapper.Map<Dictionary<string, List<EnumDTO>>>(this.customEnumBLL.GetDocumentPropertiesEnums());

            ViewBag.FilterData = dataIn;
            return View();
        }

        public ActionResult GetDocumentsPerDomain()
        {
            List<FormInstancePerDomainDataOut> result = formBLL.GetFormInstancePerDomain();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Engine)]
        public ActionResult Create()
        {
            Form form = GetFormFromRequest();
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["thesaurusId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            UserData userData = Mapper.Map<UserData>(userCookieData);

            FormInstance formInstance = GetFormInstanceSet(form);
            SetPatientRelatedData(form, formInstance, userData);

            formInstanceBLL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
            PassDataToOmnia(formInstance);

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Engine)]
        public ActionResult Create(FormInstanceFilterDataIn filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(filter.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, filter.VersionId);
            if (form == null)
            {
                return NotFound(filter.ThesaurusId.ToString());
            }

            FormDataOut data = formBLL.SetFormDependablesAndReferrals(form, null);

            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.Title = $"{Resources.TextLanguage.Create} {data.Title}";
            return View("~/Views/Form/Form.cshtml", data);
        }

        [HttpPost]
        public ActionResult CreateCTCAE([System.Web.Http.FromBody] CTCAEPatient patient)
        {
            UserData userData = Mapper.Map<UserData>(userCookieData);

            if (patient == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, patient);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Form form = formDAL.GetFormByThesaurus(15120);
            FormInstance formInstance = GetFormInstanceSet(form);

            SetFormInstanceFields(form, formInstance, patient);


            for (int i = 0; i < patient.ReviewModels.Count; i++)
            {
                SetRepetitiveFields(form, formInstance, patient.ReviewModels[i], i);
            }

            if (patient.FormInstanceId != null)
            {
                FormInstance formInstanceForUpdate = SetFormInstanceIdForUpdate(patient, formInstance);
                formInstanceDAL.InsertOrUpdate(formInstanceForUpdate, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
            }
            else
            {
                SetCTCAEPatient(form, formInstance, patient, userData);
                formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Engine)]
        public ActionResult Edit(FormInstanceFilterDataIn filter)
        {
            return GetEdit(filter);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult ExportToTxt(FormInstanceFilterDataIn filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            FormInstance formInstance = this.formInstanceDAL.GetById(filter.FormInstanceId);

            if (formInstance == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            Form form = formDAL.GetForm(formInstance.FormDefinitionId);
            form.SetFields(formInstance.Fields);
            List<FieldDataOut> fields = Mapper.Map<List<FieldDataOut>>(form.GetAllFields());
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);

            foreach (FieldDataOut formField in fields)
            {
                string value = formField.GetCSVValue(Resources.TextLanguage.N_E);
                tw.WriteLine($"{formField.Label}");
                tw.WriteLine($"{value}");
                tw.WriteLine();
            }
            tw.WriteLine("Notes:");
            tw.WriteLine($"{formInstance.Notes}");
            tw.WriteLine();
            tw.WriteLine("Date:");
            tw.WriteLine(formInstance.Date.HasValue ? formInstance.Date.Value.ToString(ViewBag.DateFormat) : "");
            tw.WriteLine();
            tw.WriteLine("Form state:");
            tw.WriteLine(Resources.TextLanguage.ResourceManager.GetString(formInstance.FormState.GetValueOrDefault(FormState.OnGoing).ToString()));
            tw.WriteLine();

            tw.Flush();
            tw.Close();

            SetCustomResponseHeaderForMultiFileDownload();

            return File(memoryStream.ToArray(), "text", formInstance.Title);
        }

        [HttpDelete]
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Engine)]
        public ActionResult Delete(string formInstanceId, DateTime lastUpdate)
        {
            formInstanceBLL.Delete(formInstanceId, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult GetFormInstanceContent(FormInstanceViewMode viewMode, string formInstanceId)
        {
            switch (viewMode)
            {
                case FormInstanceViewMode.SynopticView:
                    return GetEdit(new FormInstanceFilterDataIn { FormInstanceId = formInstanceId }, "SynopticView");
                case FormInstanceViewMode.RegularView:
                default:
                    SetReadOnlyAndDisabledViewBag(!userCookieData.UserHasPermission(PermissionNames.CreateUpdate, ModuleNames.Engine));
                    ViewBag.Action = "/FormInstance/Create";
                    return GetEdit(new FormInstanceFilterDataIn { FormInstanceId = formInstanceId }, "~/Views/Form/FormPartial.cshtml");
            }
        }

        public ActionResult GetSignDocumentModel(FormState formInstanceNextState)
        {
            return PartialView("SignDocumentModalForm", new FormInstanceSignDataIn { FormInstanceNextState = formInstanceNextState });
        }

        [SReportsAuditLog]
        [HttpPost]
        [SReportsAuthorize(Permission = PermissionNames.ChangeFormInstanceState, Module = ModuleNames.Engine)]
        public ActionResult SignDocument(FormInstanceSignDataIn formInstanceSignDataIn)
        {
            formInstanceSignDataIn = Ensure.IsNotNull(formInstanceSignDataIn, nameof(formInstanceSignDataIn));

            if (ModelState.IsValid)
            {
                var user = userBLL.IsValidUser(new DTOs.User.DataIn.UserLoginDataIn { Username = userCookieData.Username, Password = formInstanceSignDataIn.Password });
                if (user != null)
                {
                    string formInstanceId = formInstanceSignDataIn.FormInstanceId;
                    formInstanceBLL.SignDocument(formInstanceId, userCookieData.Id, formInstanceSignDataIn.FormInstanceNextState);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                }
            }
            return PartialView("SignDocumentModalForm", formInstanceSignDataIn);
        }

        [SReportsAuthorize(Permission = PermissionNames.Download, Module = ModuleNames.Engine)]
        public ActionResult ExportToCSV(string formId)
        {
            Form currentForm = formDAL.GetForm(formId);
            List<Field> allFieldCurrentForm = currentForm.GetAllFields().Where(x => !x.Id.Equals(Domain.Entities.DFD.Constants.StateSmsSystemFieldId) && !x.Id.Equals("1001")).ToList();
            // Generate header of the CSV file
            List<string> header = allFieldCurrentForm.Select(x => x.Label).ToList();
            // Prepend Patient Infos
            header = AddPatientHeaderToCsv(header);

            header.Insert(0, Resources.TextLanguage.Date_And_Time);
            if (formId == Domain.Entities.DFD.Constants.PatientGeneralInfoForm)
            {
                header.Insert(0, Resources.TextLanguage.Patient_ID);
            }

            byte[] content;
            using (var csv = new StringWriter())
            {
                csv.WriteLine(BuildRow(header));
                List<FormInstance> formInstances = formInstanceDAL.GetByAllByDefinitionId(formId);
                foreach (FormInstance formInstance in formInstances)
                {
                    List<string> rowData = new List<string>();

                    // Adding Patient Info at beginning of the row
                    rowData = AddPatientInfoToCsv(rowData, formInstance.PatientId);

                    foreach (Field field in allFieldCurrentForm)
                    {
                        List<string> fieldInstanceValues = formInstance.Fields.FirstOrDefault(x => x.Id == field.Id)?.Value;
                        rowData.Add(GetCSVCell(field, fieldInstanceValues));
                    }

                    rowData.Insert(0,formInstance.EntryDatetime.ToTimeZoned(ViewBag.UserCookieData.TimeZoneOffset as string, ViewBag.DateFormat as string));
                    if (formId == Domain.Entities.DFD.Constants.PatientGeneralInfoForm)
                    {
                        rowData.Insert(0, formInstance.Id);
                    }

                    csv.WriteLine(BuildRow(rowData));
                }

                content = Encoding.Default.GetBytes(csv.ToString());
            }

            SetCustomResponseHeaderForMultiFileDownload();
            return new FileContentResult(content, "application/csv");
        }


        private List<string> AddPatientHeaderToCsv(List<string> header)
        {
            header.Insert(0, "Date of Birth");
            header.Insert(0, "Surname");
            header.Insert(0, "Name");
            return header;
        }

        private List<string> AddPatientInfoToCsv(List<string> rowData, int patientId)
        {
            if (patientId != 0)
            {
                try
                {
                    Patient patient = patientDAL.GetById(patientId);
                    rowData.Add(patient.Name.Given != null ? patient.Name.Given : "");
                    rowData.Add(patient.Name.Family != null ? patient.Name.Family : "");
                    rowData.Add(patient.BirthDate != null ? patient.BirthDate.Value.ToString(DateConstants.DateFormat) : "");
                }
                catch (EntityCommandExecutionException)
                {
                    rowData.Add(""); rowData.Add(""); rowData.Add("");
                }

            }
            else
            {
                rowData.Add(""); rowData.Add(""); rowData.Add("");
            }
            return rowData;
        }


        private string GetCSVCell(Field field, IList<string> fieldInstanceValues)
        {
            string fieldInstanceValue = null;
            if (fieldInstanceValues != null && fieldInstanceValues.Count > 0)
            {
                fieldInstanceValue = field.GetReferrableValue(fieldInstanceValues[0]);
            }

            return fieldInstanceValue ?? string.Empty;
        }

        private string BuildRow(List<string> values)
        {

            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append(String.Format("\"{0}\",", value.SanitizeForCsvExport()));
            }

            return sb.ToString();
        }

        private void SetFields(FieldSet fieldSet, CTCAEPatient patient)
        {
            fieldSet.Fields[0].SetValue(patient.PatientId.ToString());
            fieldSet.Fields[0].InstanceId = $"{fieldSet.Id}-0-{fieldSet.Fields[0].Id}-1";
            fieldSet.Fields[1].SetValue(patient.VisitNo);
            fieldSet.Fields[1].InstanceId = $"{fieldSet.Id}-0-{fieldSet.Fields[1].Id}-1";
            fieldSet.Fields[2].SetValue(patient.Date?.ToString("yyyy-MM-dd"));
            fieldSet.Fields[2].InstanceId = $"{fieldSet.Id}-0-{fieldSet.Fields[2].Id}-1";
            fieldSet.Fields[3].SetValue(patient.Title);
            fieldSet.Fields[3].InstanceId = $"{fieldSet.Id}-0-{fieldSet.Fields[3].Id}-1";

        }

        private void SetRepetitiveFields(Form form, FormInstance formInstance, ReviewModel reviewModel, int count)
        {
            FieldSet temp = form.Chapters[0].Pages[0].ListOfFieldSets[0][0].Clone();

            formInstance.Fields.Add(new FieldValue()
            {
                Id = temp.Fields[0].Id,
                InstanceId = $"{temp.Id}-{count}-{temp.Fields[0].Id}-1",
                ThesaurusId = temp.Fields[0].ThesaurusId,
                Type = temp.Fields[0].Type,
                Value = new List<string>() { reviewModel.MedDRACode }
            });

            formInstance.Fields.Add(new FieldValue()
            {
                Id = temp.Fields[1].Id,
                InstanceId = $"{temp.Id}-{count}-{temp.Fields[1].Id}-1",
                ThesaurusId = temp.Fields[1].ThesaurusId,
                Type = temp.Fields[1].Type,
                Value = new List<string>() { reviewModel.CTCAETerms }
            });

            formInstance.Fields.Add(new FieldValue()
            {
                Id = temp.Fields[2].Id,
                InstanceId = $"{temp.Id}-{count}-{temp.Fields[2].Id}-1",
                ThesaurusId = temp.Fields[2].ThesaurusId,
                Type = temp.Fields[2].Type,
                Value = new List<string>() { ((FieldSelectable)temp.Fields[2]).Values.FirstOrDefault(x => x.Label == reviewModel.Grades).ThesaurusId.ToString() }
            });

            formInstance.Fields.Add(new FieldValue()
            {
                Id = temp.Fields[3].Id,
                InstanceId = $"{temp.Id}-{count}-{temp.Fields[3].Id}-1",
                ThesaurusId = temp.Fields[3].ThesaurusId,
                Type = temp.Fields[3].Type,
                Value = new List<string>() { reviewModel.GradeDescription }
            });
        }

        private void SetCTCAEPatient(Form form, FormInstance formInstance, CTCAEPatient patient, UserData user)
        {
            if (!form.DisablePatientData)
            {
                int patientId = 0;
                Patient patientEntity = patientDAL.GetById(patient.PatientId);
                if (patientEntity == null)
                {
                    patientEntity = new Patient();
                    patient.PatientId = 0;
                    patientEntity.Name = new Name();
                    patientEntity.PatientId = patient.PatientId;
                    patientEntity.Name.Given = "Unknown";
                    patientEntity.Name.Family = "Unknown";
                    patientEntity.Gender = Gender.Unknown;
                    patientEntity.OrganizationId = user.ActiveOrganization.GetValueOrDefault();
                    patientDAL.InsertOrUpdate(patientEntity);
                }

                formInstance.PatientId = patientEntity.PatientId;
                int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Engine", DateTime.Now, user);
                int encounterId = InsertEncounter(eocId);
                formInstance.EpisodeOfCareRef = eocId;
                formInstance.EncounterRef = encounterId;
            }
        }

        private void SetFormInstanceFields(Form form, FormInstance formInstance, CTCAEPatient patient)
        {
            formInstance.Fields = new List<FieldValue>();
            foreach (FieldSet fieldSet in form.Chapters[0].Pages[0].ListOfFieldSets[1])
            {
                SetFields(fieldSet, patient);
                formInstance.Fields.AddRange(fieldSet.Fields.Select(x => new FieldValue()
                {
                    Id = x.Id,
                    InstanceId = x.InstanceId,
                    ThesaurusId = x.ThesaurusId,
                    Type = x.Type,
                    Value = x.Value
                })
                .ToList());
            }
        }

        private FormInstance SetFormInstanceIdForUpdate(CTCAEPatient patient, FormInstance formInstance)
        {
            FormInstance formInstanceForUpdate = formInstanceDAL.GetById(patient.FormInstanceId);
            Form formForUpdate = formDAL.GetForm(formInstanceForUpdate.FormDefinitionId);
            formForUpdate.SetValuesFromReferrals(formBLL.GetFormsFromReferrals(new List<FormInstance>() { formInstance, formInstanceForUpdate }));

            foreach (List<FieldSet> fieldSets in formForUpdate.GetAllListOfFieldSets())
            {
                int j = 0;
                foreach (FieldSet fieldSet in fieldSets)
                {
                    foreach (Field field in fieldSet.Fields)
                    {
                        field.InstanceId = $"{fieldSet.Id}-{j}-{field.Id}-1";
                    }
                    j++;
                }
            }

            SetFormInstanceForUpdateFields(formInstanceForUpdate, formForUpdate);

            return formInstanceForUpdate;
        }

        private void SetFormInstanceForUpdateFields(FormInstance formInstanceForUpdate, Form formForUpdate)
        {
            formInstanceForUpdate.Fields = formForUpdate.GetAllFields().Where(x => x.Value != null).Select(x => new FieldValue()
            {
                Id = x.Id,
                ThesaurusId = x.ThesaurusId,
                InstanceId = x.InstanceId,
                Value = x.Value,
                Type = x.Type
            }).ToList();
        }

        private void PassDataToOmnia(FormInstance formInstance)
        {
            //Form: CT/22/02 (Tofacitinib) 2.0
            if (formInstance.FormDefinitionId == "630733359525c3d9eaf473da")
            {
                PassDataToOmniaDto body = GetBody(formInstance);
                Task.Run(() => GetResponse(body));
            }
        }

        private PassDataToOmniaDto GetBody(FormInstance formInstance)
        {
            IThesaurusDAL thesaurusDAL = new ThesaurusDAL(new DAL.Sql.Sql.SReportsContext());
            Form form = formBLL.GetFormById(formInstance.FormDefinitionId);
            Dictionary<string, Field> fieldDefinitions = form.GetAllFields().ToDictionary(x => x.Id, x => x);
            Dictionary<int, Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> thesaurusesFromInstance = thesaurusDAL.GetByIdsList(formInstance.Fields.Select(fI => fI.ThesaurusId).ToList()).ToDictionary(x => x.ThesaurusEntryId, x => x);
            PassDataToOmniaDto passDataToOmnia = new PassDataToOmniaDto
            {
                OmniaFieldValues = new List<OmniaFieldValue>()
            };
            foreach (var fieldValue in formInstance.Fields)
            {
                if (thesaurusesFromInstance.TryGetValue(fieldValue.ThesaurusId, out Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus))
                {
                    var codeEntity = thesaurus.Codes.FirstOrDefault(x => !x.IsDeleted);
                    if (codeEntity != null)
                    {
                        var codeName = codeEntity.Code; // or value???
                        var entredValue = fieldValue.Value?.FirstOrDefault();
                        if (entredValue != null)
                        {
                            if (fieldValue.Type == "radio")
                            {
                                if (fieldDefinitions.TryGetValue(fieldValue.Id, out Field fieldDefinition))
                                {
                                    if (fieldDefinition is FieldRadio)
                                    {
                                        entredValue = fieldDefinition.GetReferrableValue(entredValue);//FieldRadio label or value???
                                    }
                                }
                            }
                            if (entredValue != null)
                            {
                                passDataToOmnia.OmniaFieldValues.Add(new OmniaFieldValue { OmmiaVariable = codeName, Value = entredValue });
                            }
                        }
                    }

                }
            }
            return passDataToOmnia;
        }

        private IRestResponse GetResponse(object body)
        {
            string baseUrl = "https://edcdemo.wemedoo.com/DataCapture";
            string endpoint = "ReceiveFromSReports";
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest(endpoint, Method.POST);
            request.AddJsonBody(body);
            return Execute(client, request);
        }

        private IRestResponse Execute(RestClient client, RestRequest request)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse restResponse = client.Execute(request);
            HandleResponseIfNotSuccessful(restResponse);

            return restResponse;
        }

        private void HandleResponseIfNotSuccessful(IRestResponse restResponse)
        {
            if (!restResponse.IsSuccessful)
            {
                Log.Error($"Status code: {restResponse.StatusCode}, Response content: {restResponse.Content}, Response uri: {restResponse.ResponseUri}, error message: {restResponse.ErrorMessage}");
            }
        }

        public ActionResult GetFieldsToPlot(string formId)
        {
            FormDataOut formDataOut = formBLL.GetFormDataOutById(formId, userCookieData);

            List<FieldDataOut> fieldsDataOut = formBLL.GetPlottableFields(formId);

            ViewBag.FormID = formId;
            ViewBag.FormTitle = formDataOut.Title;
            ViewBag.FieldDataOutList = fieldsDataOut;
            TempData["FieldDataOutList"] = fieldsDataOut; // passing fields to GetFormInstanceFieldsById()

            return PartialView("~/Views/FormInstance/ChartFiltersPartial.cshtml");
        }

        [WebMethod]
        public ActionResult GetFormInstanceFieldsById(string formDefinitionId, List<int> fieldThesaurusIds, DateTime? DateTimeFrom, DateTime? DateTimeTo)
        {
            List<FieldDataOut> fieldsDataOut = (List<FieldDataOut>)TempData["FieldDataOutList"];
            TempData.Keep("FieldDataOutList"); // keeping data for successive requests
            DataCaptureChartUtility chartUtilityDataStructure = formInstanceBLL.GetPlottableFieldsByThesaurusId(formDefinitionId, fieldThesaurusIds, fieldsDataOut, DateTimeFrom, DateTimeTo);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(chartUtilityDataStructure),
                ContentType = "application/json"
            };
            return result;
        }

        #endregion

        //    #region Hackaton mode
        //    /** HACKATON CODE **************************************************************************************************************************/
        //    private async Task<List<FormInstance>> GetFormsAsync(bool invalidateCache=false)
        //    {
        //        List<FormInstance> forms = cache["forms"] as List<FormInstance>;
        //        if (forms == null || invalidateCache==true)
        //        {
        //            CacheItemPolicy policy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddDays(330.0) };
        //            FormInstanceCovidFilter filter = new FormInstanceCovidFilter() { ThesaurusId = "14573" };
        //            forms = await formInstanceService.GetAllFieldsByCovidFilter().ConfigureAwait(false);
        //            cache.Set("forms", forms, policy);
        //        }
        //        return forms;
        //    }

        //    private ContentResult GetResult(GFeed feed)
        //    {
        //        var serializer = new JavaScriptSerializer();
        //        var result = new ContentResult
        //        {
        //            Content = serializer.Serialize(feed.GetContent()),
        //            ContentType = "application/json"
        //        };
        //        return result;
        //    }

        //    public async void InvalidateFormsCache()
        //    {
        //        await GetFormsAsync(true).ConfigureAwait(false);
        //    }

        //    //http://localhost:55524/FormInstance/GetSelection?ThesaurusId=14583
        //    //[OutputCache(Duration=200000, VaryByParam="thesaurusId") ]
        //    public async Task<ActionResult> GetSelectionAsync(string thesaurusId)
        //    {
        //        //string thesaurusId = "14586";
        //        List<FormInstance> forms = await GetFormsAsync();
        //        FormInstance firstForm = forms[0];
        //        Domain.Entities.FieldEntity.Field field = firstForm.GetFieldByThesaurus(thesaurusId);
        //        GFeedTable t = (new GFeed(field.Label)).FirstTable;
        //        t.AddRow(new List<string>() { field.Label, field.Label });
        //        List<Domain.Entities.FieldEntity.Field> allFields = forms.SelectMany(x => x.GetAllFields().Where(f => f.ThesaurusId == thesaurusId)).ToList();

        //        if (field is Domain.Entities.FieldEntity.FieldNumeric)
        //        {
        //            CountYears(allFields, t);
        //        }
        //        else
        //        {
        //            foreach (FormFieldValue fv in (field as Domain.Entities.FieldEntity.FieldSelectable).Values)
        //            {
        //                int repCount;
        //                if (field.Type == "radio")
        //                    repCount = allFields.Count(g => g.Value?[0] == fv.ThesaurusId);
        //                else if (field.Type == "checkbox")
        //                    repCount = allFields.Count(g => g.Value.Contains(fv.Value));
        //                else
        //                    repCount = allFields.Count(g => g.Value?[0] == fv.Value);

        //                t.AddRow(new List<string>() { fv.Label, "" + repCount });
        //            }
        //        }
        //        return GetResult(t.feed);
        //    }

        //    public async Task<FileContentResult> GetData()
        //    {
        //        Dictionary<string, Dictionary<string, int>> fValues = new Dictionary<string, Dictionary<string, int>>();
        //        List<FormInstance> forms = await GetFormsAsync();

        //        StringBuilder sb = new StringBuilder();
        //        List<Domain.Entities.FieldEntity.Field> allFields2 = forms.FirstOrDefault().GetAllFields();

        //        sb.Append(",");
        //        foreach (Domain.Entities.FieldEntity.Field fld in allFields2)
        //        {
        //            sb.Append(fld.Label.Replace(","," ") + ",");
        //        }
        //        sb.Append(Environment.NewLine);
        //        //Dictionary<PatientId, Dictionary<FieldThesaurusId, AnswerVal>
        //        Dictionary<string, Dictionary<string, string>> answers = new Dictionary<string, Dictionary<string, string>>();

        //        foreach (FormInstance frm in forms)
        //        {
        //            sb.Append(frm.Id + ",");
        //            answers[frm.Id] = new Dictionary<string, string>();
        //            foreach (Domain.Entities.FieldEntity.Field fld in frm.GetAllFields())
        //            {
        //                answers[frm.Id][fld.ThesaurusId] = GetFieldVal(fld);
        //                sb.Append(GetFieldVal(fld) + ",");
        //            }
        //            sb.Append(Environment.NewLine);
        //        }

        //        return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "niAnalytics.com.csv");
        //    }

        //    //http://localhost:55524/FormInstance/GetMultiSelection?tableName=as&tableHeader=YES,NO,Unknown&thesaurusIdList=14585,14586,14584,14583
        //    //[OutputCache(Duration = 200000, VaryByParam = "tableName;tableHeader;thesaurusIdList")]
        //    public async Task<ActionResult> GetMultiSelectionAsync(string tableName, string tableHeader, string thesaurusIdList)
        //    {
        //        Dictionary<string, Dictionary<string, int>> fValues = new Dictionary<string, Dictionary<string, int>>();
        //        List<FormInstance> forms = await GetFormsAsync();

        //        /*
        //        StringBuilder sb = new StringBuilder();
        //        List<FormField> allFields2 = forms.FirstOrDefault().GetAllFields();
        //        foreach (FormField field in allFields2)
        //        {
        //            sb.Append("\"" + field.Label + "\", ");
        //            sb.Append("\"" + field.Type + "\", ");
        //            sb.Append("\"" + field.ThesaurusId + "\", ");
        //            sb.Append("\"" + GetFieldVal(field) + "\", ");
        //            sb.Append(Environment.NewLine);
        //        }
        //        */

        //        GFeedTable t = (new GFeed(tableName)).FirstTable;
        //        List<string> headerLabels = tableHeader.Split(',').ToList();
        //        headerLabels.Insert(0, tableName);
        //        t.AddRow(headerLabels);

        //        var thListTemp = thesaurusIdList.Split(',').ToList();

        //        foreach (string thId in thListTemp)
        //        {
        //            List<Domain.Entities.FieldEntity.Field> allFields = forms.SelectMany(x => x.GetAllFields().Where(f => f.ThesaurusId == thId)).ToList();
        //            foreach (Domain.Entities.FieldEntity.Field field in allFields)
        //            {
        //                if (!fValues.ContainsKey(field.Label))
        //                    fValues[field.Label] = new Dictionary<string, int>();

        //                string fieldValue = GetFieldVal(field);
        //                List<string> fieldValues = new List<string>() { fieldValue };
        //                if (field.Type == "checkbox")
        //                {
        //                    fieldValues = fieldValue.Split(',').ToList();
        //                }
        //                foreach (string val in fieldValues)
        //                {
        //                    if (!fValues[field.Label].ContainsKey(val))
        //                        fValues[field.Label][val] = 0;
        //                    fValues[field.Label][val] = fValues[field.Label][val] + 1;
        //                }
        //            }
        //        }

        //        foreach (string k in fValues.Keys)
        //        {
        //            List<string> row = new List<string>();
        //            row.Add(k);
        //            for (int i = 1; i < headerLabels.Count; i++)
        //            {
        //                string headerL = headerLabels[i];
        //                if (fValues[k].ContainsKey(headerL))
        //                    row.Add("" + fValues[k][headerL]);
        //                else
        //                    row.Add("0");
        //            }
        //            t.AddRow(row);
        //        }

        //        return GetResult(t.feed);
        //    }


        //    private string GetFieldVal(Field field)
        //    {
        //        if (field.Value == null || string.IsNullOrEmpty(field.Value[0]))
        //            return "";

        //        if (field is FieldRadio)
        //            return (field as FieldRadio).Values.FirstOrDefault(v => v.ThesaurusId == field.Value[0]).Label;
        //        else
        //            return field.Value[0];
        //    }

        //    private void CountYears(List<Domain.Entities.FieldEntity.Field> allFields, GFeedTable t)
        //    {
        //        Dictionary<string, int> yearsDistribution = new Dictionary<string, int>();
        //        yearsDistribution.Add("0-10", 0);
        //        yearsDistribution.Add("10-20", 0);
        //        yearsDistribution.Add("20-30", 0);
        //        yearsDistribution.Add("30-40", 0);
        //        yearsDistribution.Add("40-50", 0);
        //        yearsDistribution.Add("50-60", 0);
        //        yearsDistribution.Add("60-70", 0);
        //        yearsDistribution.Add("70-80", 0);
        //        yearsDistribution.Add("80-90", 0);
        //        yearsDistribution.Add("90-100", 0);
        //        yearsDistribution.Add("100+", 0);

        //        foreach (Domain.Entities.FieldEntity.Field f in allFields)
        //        {
        //            decimal years;
        //            if (decimal.TryParse(f.Value?[0], out years))
        //            {
        //                if (years <= 10) yearsDistribution["0-10"]++;
        //                else if (years > 10 && years <= 20) yearsDistribution["10-20"]++;
        //                else if (years > 20 && years <= 30) yearsDistribution["20-30"]++;
        //                else if (years > 30 && years <= 40) yearsDistribution["30-40"]++;
        //                else if (years > 40 && years <= 50) yearsDistribution["40-50"]++;
        //                else if (years > 50 && years <= 60) yearsDistribution["50-60"]++;
        //                else if (years > 60 && years <= 70) yearsDistribution["60-70"]++;
        //                else if (years > 70 && years <= 80) yearsDistribution["70-80"]++;
        //                else if (years > 80 && years <= 90) yearsDistribution["80-90"]++;
        //                else if (years > 90 && years <= 100) yearsDistribution["90-100"]++;
        //                else yearsDistribution["100+"]++;
        //            }
        //        }
        //        foreach (string k in yearsDistribution.Keys)
        //        {
        //            t.AddRow(new List<string>() { k, "" + yearsDistribution[k] });
        //        }
        //    }
        //}


        //public class GFeedTable
        //{
        //    public GFeed feed { get; set; }
        //    public List<List<string>> tContent = new List<List<string>>();

        //    public List<string> AddRow(List<string> colValues)
        //    {
        //        tContent.Add(colValues);
        //        return colValues;
        //    }

        //    public List<List<string>> GetRows()
        //    {
        //        return tContent;
        //    }
        //}

        //public class GFeed
        //{
        //    public Dictionary<string, GFeedTable> tables = new Dictionary<string, GFeedTable>();

        //    public GFeedTable FirstTable { get; set; }

        //    public GFeed(string firstTableName)
        //    {
        //        FirstTable = AddTable(firstTableName);
        //    }

        //    public GFeedTable GetTable(string tableName)
        //    {
        //        return tables[tableName];
        //    }

        //    public GFeedTable AddTable(string tableName)
        //    {
        //        tables[tableName] = new GFeedTable();
        //        tables[tableName].feed = this;
        //        return tables[tableName];
        //    }

        //    public object GetContent()
        //    {
        //        List<List<List<string>>> retVal = new List<List<List<string>>>();
        //        foreach (string k in tables.Keys)
        //        {
        //            retVal.Add(tables[k].GetRows());
        //        }
        //        return retVal;
        //    }
        //    #endregion
    }
    
}
