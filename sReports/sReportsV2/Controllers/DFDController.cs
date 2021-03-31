using AutoMapper;
using DfD.SMSApi.Client;
using DfD.SMSApi.Client.DTOs.DataIn;
using DfD.SMSApi.Client.DTOs.DataOut;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.DFD;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DfD.DataIn;
using sReportsV2.DTOs.DfD.DfDDataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static sReportsV2.Domain.Entities.FormInstance.FormInstance;

namespace sReportsV2.Controllers
{
    public class DFDController : SimplifiedPageController
    {
        private readonly IDFDService dFDService;
        private readonly SMSApiClient client;
        public DFDController(){
            dFDService = new DFDService();
            client = new SMSApiClient(ConfigurationManager.AppSettings["dfdSmsApi"], ConfigurationManager.AppSettings["dfdSmsApiKey"]);

        }

        // GET: DFD
        [SReportsAuditLog]
        public ActionResult Create(string id, string patientId, string token)
        {
            if (id.Equals(Constants.PatientGeneralInfoForm) && !IsTokenValid(token)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Form form = formService.GetForm(id);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, id);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (!string.IsNullOrWhiteSpace(patientId))
            {
                if (!formInstanceService.ExistsById(patientId)) 
                {
                    Log.Warning("Doesn't exist form value with the requested id (patient id)", 404, patientId);
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                form.SetFieldValue(Constants.PatientIdentificatorFieldId, patientId);
            }

            FormDataOut data = Mapper.Map<FormDataOut>(form);
            SetDependables(form, data.GetAllFields());

            ViewBag.Token = token;
            return View(data);
        }

        [HttpPost]
        [SReportsAuditLog]
        public ActionResult Create(string token)
        {
            Form form = this.formService.GetForm(Request.Form["formDefinitionId"]);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["formDefinitionId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FormInstance formInstance = GetFormInstanceSet(form);
            formInstance.SetFieldValue(Constants.StateSmsSystemFieldId, new List<string> { SmsState.WaitingForVerify.ToString() });
            formInstanceService.InsertOrUpdate(formInstance);
            formInstance.SetFieldValue(Constants.PatientControlCodeFieldId, new List<string>() { token ?? string.Empty });
            Task.Run(() => SendRequestToSMSApi(formInstance, form.Id, token));
            //Task.Run(() => InvalidateToken(token));
            Task.Run(() => NotifyIfCritical(formInstance));

            return RedirectToAction("InfoMesasge");
        }

        public ActionResult InfoMesasge()
        {
            return View();
        }

        public ActionResult GetByPatient(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Notify()
        {
            /*List<FormInstance> formValues = this.formInstanceService.GetAllFormsByFieldIdAndValue(Constants.StateSmsSystemFieldId, SmsState.WaitingForVerify.ToString());
            foreach(FormInstance formInstance in formValues)
            {
                Task.Run(() => SendRequestToSMSApi(formInstance, formInstance.FormDefinitionId));                
            }*/

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private async void NotifyIfCritical(FormInstance formInstance)
        {
            if (formInstance.FormDefinitionId.Equals(Constants.StressEvaluationFormId))
            {
                bool isDassCritical = IsDassCritical(formInstance);
                bool isQoLCritical = IsQualityOfLifeCritical(formInstance);
                if(isDassCritical && isQoLCritical)
                {
                    var json = new { id = formInstance.GetFieldValueById(Constants.PatientIdentificatorFieldId) };
                    await client.NotifySMSAPI(ConfigurationManager.AppSettings["dfdSmsApiSendWarningMessage"], JsonConvert.SerializeObject(json)).ConfigureAwait(false);

                }
            }
        }

        private bool IsQualityOfLifeCritical(FormInstance formInstance)
        {
            bool result = false;

            List<Field> fields = formInstance.GetAllFieldSets(formService.GetForm(formInstance.FormDefinitionId)).FirstOrDefault(x => x.Id.Equals(Constants.QoLFormCovidFieldSetId)).Fields;
            var groupedByValue = fields.GroupBy(x => x.Value != null && x.Value.Count > 0 ? x.Value[0] : null ).Select(x => new { x.Key, Count = x.Count()});
            int yesValues = groupedByValue.FirstOrDefault(x => x.Key != null && x.Key.Equals(Constants.QoLFormYesFieldValueThesaurusId))?.Count ?? 0;
            int mainlyYesValues = groupedByValue.FirstOrDefault(x => x.Key != null && x.Key.Equals(Constants.QoLFormMainlyYesFieldValueThesaurusId))?.Count ?? 0;


            if(yesValues > 0 || mainlyYesValues > 2)
            {
                result = true;
            }
            return result;
        }

        private bool IsDassCritical(FormInstance formInstance)
        {
            bool result = false;

            FieldSet fieldSet = formInstance.GetAllFieldSets(formService.GetForm(formInstance.FormDefinitionId)).FirstOrDefault(x => x.Id.Equals(Constants.Dass21FieldSetId));
            List<Field> dasFields = fieldSet.Fields;

            double? das21Total = GetDasNumericValue(dasFields, Constants.Dass21TotalFieldId);
            double? das21Depression = GetDasNumericValue(dasFields, Constants.Dass21DFieldId);
            double? das21A = GetDasNumericValue(dasFields, Constants.Dass21AFieldId);
            double? das21Stress = GetDasNumericValue(dasFields, Constants.Dass21SFieldId);

            if (das21Total != null && das21Total > 30)
            {
                result = true;
            }
            else if (das21Stress != null && das21Stress > 20)
            {
                result = true;
            }
            else if (das21A != null && das21A > 12)
            {
                result = true;
            }
            else if (das21Depression != null && das21Depression > 12)
            {
                result = true;
            }

            return result;
        }

        private double? GetDasNumericValue(List<Field> dasFields, string fieldId)
        {
            string stringValue = dasFields.FirstOrDefault(x => x.Id.Equals(fieldId))?.Value?[0];
            return  string.IsNullOrWhiteSpace(stringValue) ? null : (double?)double.Parse(stringValue);
        }

        private bool IsTokenValid(string token)
        {
            if(!Guid.TryParse(token, out Guid guidOutput))
            {
                return false;
            }

            ValidationResultDataIn validationResult = client.ValidateToken(ConfigurationManager.AppSettings["dfdTokenValidation"], token);
            return validationResult.StatusCode == ValidationStatus.Valid;
        }

        private async void InvalidateToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                InvalidateTokenDataOut invalidateTokenDataOut = new InvalidateTokenDataOut()
                {
                    Token = token
                };

                await client.InvalidateToken(ConfigurationManager.AppSettings["dfdTokenInvalidation"], JsonConvert.SerializeObject(invalidateTokenDataOut)).ConfigureAwait(false);
            }
        }
        

        private void SendRequestToSMSApi(FormInstance formInstance, string formId, string token)
        {
            Log.Information("Send confirmation request.");

            if (formId == Constants.PatientGeneralInfoForm)
            {
                NotifyPatientSubmitted(formInstance, token);
            }
            else
            {
                string patientId = formInstance.Fields.FirstOrDefault(x => x.Id == Constants.PatientIdentificatorFieldId)?.Value[0];
                NotifySurveySubmittedSMSSystem(formInstance, patientId);
            }
        }

        private async void NotifyPatientSubmitted(FormInstance formInstance, string token)
        {
            DoctorDataOut doctorDataOut = new DoctorDataOut()
            {
                id = formInstance.Id,
                invalidationToken = token,
                gender = GetGender(formInstance.GetFieldValueById(Constants.PatintGenderFieldId))
            };

            bool notified = await client.NotifySMSAPI(ConfigurationManager.AppSettings["dfdSmsApiUserCreatedNewUrl"], JsonConvert.SerializeObject(doctorDataOut)).ConfigureAwait(false);
            Log.Information($"Is SMS API notified: {notified}");
            if (notified)
            {
                Log.Information($"Token invalidated:{token}patient-{doctorDataOut.id}");
                Log.Information($"User created: {doctorDataOut.id}");
                UpdateDocumentNotifiedFlag(formInstance);
            }
        }

        private async void NotifySurveySubmittedSMSSystem(FormInstance formInstance, string patientId)
        {
            SurveySubmittedDataOut surveySubmittedDataOut = new SurveySubmittedDataOut()
            {
                submittedDateTime = formInstance.EntryDatetime.ToUniversalTime().ToString("o"),
                surveyId = formInstance.FormDefinitionId,
                userId = patientId
            };

            bool notified = await client.NotifySMSAPI(ConfigurationManager.AppSettings["dfdSmsApiConfirmationUrl"], JsonConvert.SerializeObject(surveySubmittedDataOut)).ConfigureAwait(false);
            if (notified)
            {
                UpdateDocumentNotifiedFlag(formInstance);
            }
        }
        
        private void UpdateDocumentNotifiedFlag(FormInstance formInstance)
        {
            formInstance.SetFieldValue(Constants.StateSmsSystemFieldId, new List<string> { SmsState.Notified.ToString() });
            formInstanceService.InsertOrUpdate(formInstance);
        }

        private string GetGender(string value)
        {
            string result = "UNKNOWN";
            if (value == Constants.PatientGenderMaleThesaurusId)
            {
                result = "MALE";
            }
            else if (value == Constants.PatientGenderFemaleThesaurusId)
            {
                result = "FEMALE";
            }

            return result;
        }
    }
}