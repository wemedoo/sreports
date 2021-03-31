using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DiagnosticReport;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportController : FormCommonController
    {

        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        public DiagnosticReportController()
        {
            organizationService = new OrganizationService();
            userService = new UserService();
        }

        public ActionResult ReloadTable(DiagnosticReportFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            List<FormInstance> instances = this.formInstanceService.GetByEpisodeOfCare(dataIn.EpisodeOfCareId, dataIn.PageSize, dataIn.Page);
            PaginationDataOut<DiagnosticReportDataOut, DiagnosticReportFilterDataIn> result = new PaginationDataOut<DiagnosticReportDataOut, DiagnosticReportFilterDataIn>()
            {
                Count = this.formInstanceService.CountEpisodeOfCare(dataIn.EpisodeOfCareId),
                DataIn = dataIn,
                Data = Mapper.Map<List<DiagnosticReportDataOut>>(instances)
            };

            return PartialView("DiagnosticReportsList", result);
        }

        public ActionResult ModalListForms(string encounterId, string episodeOfCareId, List<string> referrals)
        {
            if (string.IsNullOrWhiteSpace(episodeOfCareId)) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please choose episode of care!");
            }


            return PartialView(GetEpisodeOfCareListFormsDataOut(episodeOfCareId, referrals, encounterId));
        }

        public ActionResult ListForms(string encounterId, string episodeOfCareId, List<string> referrals)
        {
           
            return View(GetEpisodeOfCareListFormsDataOut(episodeOfCareId, referrals, encounterId));
        }

        public ActionResult Create(string encounterId, string episodeOfCareId, string formId, List<string> referrals)
        {
            DiagnosticReportCreateDataOut diagnosticReportCreateDataOut = GetDiagnosticReportDataOut(episodeOfCareId, formId, referrals);
            ViewBag.EncounterId = encounterId;
            return View(diagnosticReportCreateDataOut);
        }

        public ActionResult CreateFromPatient(string encounterId, string episodeOfCareId, string formId, List<string> referrals)
        {
            DiagnosticReportCreateDataOut diagnosticReportCreateDataOut = GetDiagnosticReportDataOut(episodeOfCareId, formId, referrals);
            ViewBag.EncounterId = encounterId;
            ViewBag.Action = $"/DiagnosticReport/CreateFromPatient?episodeOfCareId={episodeOfCareId}";
            ViewBag.Referrals = diagnosticReportCreateDataOut.Referrals;
            return PartialView("~/Views/Form/FormPartial.cshtml", diagnosticReportCreateDataOut.Form);
        }

        [SReportsAuditLog]
        [Authorize]
        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult CreateFromPatient(string episodeOfCareId)
        {
            ActionResult postResult = PostDiagnosticReport(episodeOfCareId);
            if (postResult != null) 
            {
                return postResult;
            }
            
            return RedirectToAction("Edit", "Patient", new { PatientId = episodeOfCareService.GetEOCById(episodeOfCareId).PatientId });
        }

        [SReportsAuditLog]
        [Authorize]
        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult Create(string episodeOfCareId)
        {
            ActionResult postResult = PostDiagnosticReport(episodeOfCareId);

            if (postResult != null)
            {
                return postResult;
            }

            return RedirectToAction("Edit", "EpisodeOfCare", new { EpisodeOfCareId = episodeOfCareId });
        }

        [SReportsAuditLog]
        [Authorize]
        public ActionResult EditFromPatient(string formInstanceId)
        {
            formInstanceId = Ensure.IsNotNull(formInstanceId, nameof(formInstanceId));

            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            if (formInstance == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, formInstanceId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            FormDataOut data = GetFormDataOut(formInstance, formInstanceService.GetByIds(formInstance.Referrals).ToList());

            ViewBag.FormInstanceId = formInstanceId;
            ViewBag.Title = data.Title;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.Referrals = formInstance.Referrals != null ? Mapper.Map<List<FormInstanceReferralDataOut>>(this.formInstanceService.GetByIds(formInstance.Referrals)) : new List<FormInstanceReferralDataOut>();
            ViewBag.Action = $"/DiagnosticReport/Create?episodeOfCareId={encounterService.GetById(formInstance.EncounterRef).EpisodeOfCareId}";

            return PartialView("~/Views/Form/FormPartial.cshtml", data);
        }

        [SReportsAuditLog]
        [Authorize]
        public ActionResult Edit(string encounterId, string episodeOfCareId, string diagnosticReportId)
        {
            ViewBag.EncounterId = encounterId;
            FormInstance formInstance = this.formInstanceService.GetById(diagnosticReportId);
            if (formInstance == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, diagnosticReportId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return GetEdit(formInstance, episodeOfCareId);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string diagnosticReportId, DateTime lastUpdate)
        {
            try
            {
                formInstanceService.Delete(diagnosticReportId, lastUpdate);
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

        private ActionResult GetEdit(FormInstance formInstance, string episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareService.GetEOCById(episodeOfCareId));
            List<FormInstance> referrals = formInstanceService.GetByIds(formInstance.Referrals).ToList();
            DiagnosticReportCreateDataOut diagnosticReportCreateDataOut = new DiagnosticReportCreateDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Patient = Mapper.Map<PatientDataOut>(patientService.GetById(episodeOfCareDataOut.PatientId)),
                Referrals = formInstance.Referrals != null ? Mapper.Map<List<FormInstanceReferralDataOut>>(referrals) : null
            };

            diagnosticReportCreateDataOut.Form = GetFormDataOut(formInstance, referrals);

            ViewBag.FormInstanceId = formInstance.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.LastUpdate = formInstance.LastUpdate;


            return View("Create", diagnosticReportCreateDataOut);
        }

        private DiagnosticReportCreateDataOut GetDiagnosticReportDataOut(string episodeOfCareId, string formId, List<string> referrals)
        {
            Form form = this.formService.GetForm(formId);
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareService.GetEOCById(episodeOfCareId));
            List<FormInstance> formInstancesReferrals = referrals != null ? formInstanceService.GetByIds(referrals).ToList() : new List<FormInstance>();
            List<Form> formReferrals = GetFormsFromReferrals(formInstancesReferrals);
            form.SetValuesFromReferrals(formReferrals);
            DiagnosticReportCreateDataOut diagnosticReportCreateDataOut = new DiagnosticReportCreateDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Patient = Mapper.Map<PatientDataOut>(patientService.GetById(episodeOfCareDataOut.PatientId)),
                Form = Mapper.Map<FormDataOut>(form),
                Referrals = Mapper.Map<List<FormInstanceReferralDataOut>>(formInstancesReferrals)
            };
            diagnosticReportCreateDataOut.Form.SetDependables();
            diagnosticReportCreateDataOut.Form.ReferrableFields = GetReferrableFields(form, formReferrals);

            return diagnosticReportCreateDataOut;
        }

        private EpisodeOfCareListFormsDataOut GetEpisodeOfCareListFormsDataOut(string episodeOfCareId, List<string> referrals, string encounterId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareService.GetEOCById(episodeOfCareId));

            EpisodeOfCareListFormsDataOut episodeOfCareListFormsDataOut = new EpisodeOfCareListFormsDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Forms = Mapper.Map<List<FormEpisodeOfCareDataOut>>(formService.GetAllByOrganizationAndLanguage(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage)),
                Patient = Mapper.Map<PatientDataOut>(patientService.GetById(episodeOfCareDataOut.PatientId)),
                Referrals = referrals,
                EncounterId = encounterId

            };

            return episodeOfCareListFormsDataOut;
        }

        private ActionResult PostDiagnosticReport(string episodeOfCareId) 
        {
            Form form = this.formService.GetForm(Request.Form["formDefinitionId"]);
            string encounterId = Request.Form["encounterId"];
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["formDefinitionId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FormInstance formInstance = GetFormInstanceSet(form);

            if (string.IsNullOrWhiteSpace(encounterId))
            {
                encounterId = InsertEncounter(episodeOfCareId);
            }
            formInstance.EncounterRef = encounterId;
            formInstance.EpisodeOfCareRef = episodeOfCareId;
            formInstance.PatientRef = episodeOfCareService.GetEOCById(episodeOfCareId).PatientId;

            try
            {
                formInstanceService.InsertOrUpdate(formInstance);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                ModelState.AddModelError("ConcurrencyException", message);
                Log.Error(ex.Message);

                return GetEdit(formInstance, episodeOfCareId);
            }

            return null;
        }
    }
}