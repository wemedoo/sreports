using AutoMapper;
using Chapters;
using RestSharp;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient.DataOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.DTOs;
using sReportsV2.Common.Extensions;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;

namespace sReportsV2.Controllers
{
    //[Authorize]
    public class FormInstanceController : FormCommonController
    {

        private static ObjectCache cache = MemoryCache.Default;

        #region Before Hackaton
        public FormInstanceController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL,IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL ,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
        }

        [SReportsAuditLog]
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
            }; ;
            //return Json(await formInstanceService.GetAllFieldsByCovidFilter().ConfigureAwait(false), JsonRequestBehavior.AllowGet, JsonRequestBehavior);
        }

        //[Authorize]
        [SReportsAutorize]
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
        [SReportsAutorize]
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
        [SReportsAutorize]
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
            try
            {
                formInstanceBLL.InsertOrUpdate(formInstance);
            }
            catch (MongoDbConcurrencyException ex)
            {
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, Resources.TextLanguage.ConcurrencyExEdit);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult Create(FormInstanceFilterDataIn filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            ViewBag.FilterFormInstanceDataIn = filter;

            Form form = formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(filter.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, filter.VersionId);
            if (form == null)
            {
                return NotFound(filter.ThesaurusId.ToString());
            }

            FormDataOut data = formBLL.SetFormDependablesAndReferrals(form, null);

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


            for(int i = 0; i < patient.ReviewModels.Count; i++)
            {
                SetRepetitiveFields(form, formInstance, patient.ReviewModels[i], i);
            }

            if (patient.FormInstanceId != null)
            {
                FormInstance formInstanceForUpdate = SetFormInstanceIdForUpdate(patient, formInstance);
                formInstanceDAL.InsertOrUpdate(formInstanceForUpdate);
            }
            else
            {
                SetCTCAEPatient(form, formInstance, patient, userData);
                formInstanceDAL.InsertOrUpdate(formInstance);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult Edit(FormInstanceFilterDataIn filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            FormInstance formInstance = formInstanceBLL.GetById(filter.FormInstanceId);
            if (formInstance == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, filter.FormInstanceId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.Title = formInstance.Title;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;

            return GetEdit(formInstance, filter);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
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
                string value;
                if (formField.Value != null && formField.Value.Count > 0)
                    value = formField.GetValue();
                else
                    value = "";

                tw.WriteLine($"{formField.Label}");
                tw.WriteLine($"{value}");
                tw.WriteLine();
            }
            tw.WriteLine("Notes:");
            tw.WriteLine($"{formInstance.Notes}");
            tw.WriteLine();
            tw.WriteLine("Date:");
            tw.WriteLine($"{formInstance.Date}");
            tw.WriteLine();
            tw.WriteLine("Form state:");
            tw.WriteLine($"{formInstance.FormState}");
            tw.WriteLine();

            tw.Flush();
            tw.Close();

            return File(memoryStream.ToArray(), "text", formInstance.Title);
        }

        [HttpDelete]
        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult Delete(string formInstanceId, DateTime lastUpdate)
        {
            try
            {
                formInstanceBLL.Delete(formInstanceId, lastUpdate);
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

        public ActionResult ExportToCSV(string formId)
        {
            Form currentForm = formDAL.GetForm(formId);
            List<Field> allFieldCurrentForm = currentForm.GetAllFields().Where(x => !x.Id.Equals(Domain.Entities.DFD.Constants.StateSmsSystemFieldId) && !x.Id.Equals("1001")).ToList();
            List<string> header = allFieldCurrentForm.Select(x => x.Label).ToList();


            header.Insert(0, "Datume i vreme");
            if(formId == Domain.Entities.DFD.Constants.PatientGeneralInfoForm)
            {
                header.Insert(0, "Identifikator pacijenta");
            }


            byte[] content;
            using (var csv = new StringWriter())
            {
                csv.WriteLine(BuildRow(header));

                // Generate header of the CSV file
                List<FormInstance> values = formInstanceDAL.GetByAllByDefinitionId(formId);
                foreach (FormInstance formValue in values)
                {
                    currentForm.SetFieldsValuesFromFormValue(formValue);
                    List<string> rowData = new List<string>();
                    foreach (Field field in allFieldCurrentForm)
                    {
                        rowData.Add(GetCSVCell(field));
                    }

                    rowData.Insert(0,formValue.EntryDatetime.ToString("yyyy-MM-dd HH:mm"));
                    if (formId == Domain.Entities.DFD.Constants.PatientGeneralInfoForm)
                    {
                        rowData.Insert(0, formValue.Id);
                    }

                    csv.WriteLine(BuildRow(rowData));                   
                }

                content = Encoding.Default.GetBytes(csv.ToString());
            }

            return new FileContentResult(content, "application/csv");
        }

        private string GetCSVCell(Field field)
        {
            string result = string.Empty;
            if (field.Value != null && field.Value.Count > 0)
            {
                if (field is FieldRadio)
                {
                    FormFieldValue fieldValue = (field as FieldRadio).Values.FirstOrDefault(x => x.ThesaurusId.Equals(field.Value[0]));
                    if (fieldValue != null)
                    {
                        result = fieldValue.Label;
                    }
                }
                else
                {
                    result = field.Value[0];
                }
            }

            return result;
        }

        private string BuildRow(List<string> values)
        {

            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                if (value.Contains(","))
                {
                    sb.Append(String.Format("\"{0}\",", value));
                }
                else
                {
                    sb.Append(String.Format("{0},", value));
                }
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
                Value =new List<string>() { reviewModel.MedDRACode }
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
                Value =new List<string>() { ((FieldSelectable)temp.Fields[2]).Values.FirstOrDefault(x => x.Label == reviewModel.Grades).ThesaurusId.ToString() }
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
                bool existPatient = patientDAL.ExistsPatientByObjectId(patient.PatientId);
                Patient patientEntity = new Patient();

                if (!existPatient)
                {
                    patient.PatientId = 0;
                    patientEntity.Name = new Name();
                    patientEntity.Addresss = new Address();
                    patientEntity.Id = patient.PatientId;
                    patientEntity.Name.Given = "Unknown";
                    patientEntity.Name.Family = "Unknown";
                    patientEntity.Gender = Gender.Unknown;
                    patientEntity.Addresss.City = "Unknown";
                }
                else 
                {
                    patientEntity = patientDAL.GetById(patient.PatientId);
                }

                int patientId = patientDAL.Insert(patientEntity);
                formInstance.PatientId = patientId;
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
