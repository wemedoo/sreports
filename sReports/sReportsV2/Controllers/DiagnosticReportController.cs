﻿using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.DTOs.DiagnosticReport.DataOut;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportController : DiagnosticReportCommonController
    {
        public DiagnosticReportController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL) { }

        public ActionResult ListForms(string encounterId, int episodeOfCareId, List<string> referrals)
        {
            return View(GetEpisodeOfCareListFormsDataOut(episodeOfCareId, referrals, encounterId));
        }
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Patients)]
        public ActionResult CreateFromPatient(string patientId, int encounterId, string episodeOfCareId, string formId, List<string> referrals)
        {
            Form form = formDAL.GetForm(formId);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, formId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }


            List<Form> formReferrals = GetRefeerals(referrals);
            form.SetValuesFromReferrals(formReferrals);

            FormDataOut formOut = formBLL.SetFormDependablesAndReferrals(form, formReferrals);
           
            Encounter encounter = encounterDAL.GetById(encounterId);
            DiagnosticReportCreateFromPatientDataOut result = new DiagnosticReportCreateFromPatientDataOut()
            {
                CurrentForm = formOut,
                Encounter = Mapper.Map<EncounterDataOut>(encounter),
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(this.formInstanceDAL.GetAllByEncounter(encounterId, userCookieData.ActiveOrganization))
            };
            ViewBag.EncounterId = encounterId;
            ViewBag.Action = $"/DiagnosticReport/CreateFromPatient?episodeOfCareId={episodeOfCareId}&patientId={patientId}";
            ViewBag.Referrals = referrals;
            return PartialView("CreateFromPatient", result);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult EditFromPatient(string formInstanceId, int encounterId)
        {
             formInstanceId = Ensure.IsNotNull(formInstanceId, nameof(formInstanceId));

            FormInstance formInstance = formInstanceDAL.GetById(formInstanceId);
            if (formInstance == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, formInstanceId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            List<FormInstance> referrals = formInstanceBLL.GetByIds(formInstance.Referrals);
            FormDataOut data = formBLL.GetFormDataOut(formInstance, referrals, userCookieData);
            Encounter encounter = encounterDAL.GetById(formInstance.EncounterRef);

            DiagnosticReportCreateFromPatientDataOut diagnosticReportCreateFromPatientDataOut = new DiagnosticReportCreateFromPatientDataOut()
            {
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(this.formInstanceDAL.GetAllByEncounter(encounterId, userCookieData.ActiveOrganization)),
                Encounter = Mapper.Map<EncounterDataOut>(encounter),
                CurrentForm = data
            };


            ViewBag.FormInstanceId = formInstanceId;
            ViewBag.Title = data.Title;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.VersionId = formInstance.Version.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.Referrals = formInstance.Referrals;
            ViewBag.Action = $"/DiagnosticReport/CreateFromPatient?episodeOfCareId={encounter.EpisodeOfCareId}&patientId={encounter.PatientId}";
            SetReadOnlyAndDisabledViewBag(!ViewBag.UserCookieData.UserHasPermission(PermissionNames.CreateUpdate, ModuleNames.Patients));
            return PartialView("CreateFromPatient", diagnosticReportCreateFromPatientDataOut);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Patients)]
        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult CreateFromPatient(int episodeOfCareId, int patientId)
        {
            Form form = this.formDAL.GetForm(Request.Form["formDefinitionId"]);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["formDefinitionId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FormInstance formInstance = GetFormInstanceSet(form);
            formInstance.EncounterRef = GetEncounterFromRequestOrCreateDefault(episodeOfCareId);
            formInstance.EpisodeOfCareRef = episodeOfCareId;
            formInstance.PatientId = patientId;

            formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(userCookieData?.Id));

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuthorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string diagnosticReportId, DateTime lastUpdate)
        {
            formInstanceDAL.Delete(diagnosticReportId, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}