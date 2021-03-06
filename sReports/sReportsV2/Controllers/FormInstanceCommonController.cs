using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Common.Extensions;
using Chapters;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;

namespace sReportsV2.Controllers
{
    public partial class FormCommonController
    {
        protected int GetEncounterFromRequestOrCreateDefault(int episodeOfCareId)
        {
            int result = Int32.Parse(Request.Form["encounterId"]);
            if (result == 0)
            {
                result = InsertEncounter(episodeOfCareId);
            }

            return result;
        }

        protected Form GetFormFromRequest()
        {
            string versionId = string.IsNullOrWhiteSpace(Request.Form["EditVersionId"]) ? Request.Form["VersionId"] : Request.Form["EditVersionId"];
            return this.formBLL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(Int32.Parse(Request.Form["thesaurusId"]), userCookieData.ActiveOrganization, Request.Form["language"], versionId);
        }

        protected ActionResult GetEdit(FormInstance formInstance, FormInstanceFilterDataIn filter)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));
            filter = Ensure.IsNotNull(filter, nameof(filter));
            List<FormInstance> referrals = this.formInstanceBLL.GetByIds(formInstance.Referrals != null ? formInstance.Referrals : new List<string>());
            FormDataOut data = formBLL.GetFormDataOut(formInstance, referrals, userCookieData);

            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.Referrals = referrals != null && referrals.Count > 0 ? referrals.Select(x => x.Id) : null;
            

            return View("~/Views/Form/Form.cshtml", data);
        }

        protected FormInstanceFilterDataIn GetFormInstaceFilter(HttpRequestBase request, FormInstance formInstance, string formId)
        {
            FormInstanceFilterDataIn filter = new FormInstanceFilterDataIn();
            filter.FormId = formId;
            filter.FormInstanceId = formInstance.Id;
            filter.Title = formInstance.Title;
            filter.VersionId = request.Form["VersionId"];

            if (!string.IsNullOrEmpty(request.Form["DocumentClass"]))
            {
                _ = Enum.TryParse(request.Form["DocumentClass"], out DocumentClassEnum documentClass);
                filter.DocumentClass = documentClass;
            }

            if (!string.IsNullOrEmpty(request.Form["FollowUp"]))
            {
                _ = Enum.TryParse(request.Form["FollowUp"], out FollowUp followUp);
                filter.FollowUp = followUp;
            }

            filter.ClassesOtherValue = request.Form["ClassesOtherValue"];

            if (!string.IsNullOrEmpty(request.Form["GeneralPurpose"]))
            {
                _ = Enum.TryParse(request.Form["GeneralPurpose"], out DocumentGeneralPurposeEnum generalPurpose);
                filter.GeneralPurpose = generalPurpose;
            }

            if (!string.IsNullOrEmpty(request.Form["ExplicitPurpose"]))
            {
                _ = Enum.TryParse(request.Form["ExplicitPurpose"], out DocumentExplicitPurpose explicitPurpose);
                filter.ExplicitPurpose = explicitPurpose;
            }

            if (!string.IsNullOrEmpty(request.Form["ScopeOfValidity"]))
            {
                _ = Enum.TryParse(request.Form["ScopeOfValidity"], out DocumentScopeOfValidityEnum scopeOfValidity);
                filter.ScopeOfValidity = scopeOfValidity;
            }

            if (!string.IsNullOrEmpty(request.Form["ClinicalDomain"]))
            {
                _ = Enum.TryParse(request.Form["ClinicalDomain"], out DocumentClinicalDomain clinicalDomain);
                filter.ClinicalDomain = clinicalDomain;
            }

            if (!string.IsNullOrEmpty(request.Form["ClinicalContext"]))
            {
                _ = Enum.TryParse(request.Form["ClinicalContext"], out DocumentClinicalContextEnum clinicalContext);
                filter.ClinicalContext = clinicalContext;
            }

            if (!string.IsNullOrEmpty(request.Form["AdministrativeContext"]))
            {
                _ = Enum.TryParse(request.Form["AdministrativeContext"], out AdministrativeContext administrativeContext);
                filter.AdministrativeContext = administrativeContext;
            }

            filter.ThesaurusId = Int32.Parse(Request.Form["thesaurusId"]);

            return filter;
        }

        #region PatientRelatedData
        protected void SetPatientRelatedData(Form form, FormInstance formInstance, UserData user)
        {
            int encounterId = Request.Form["encounterId"] != null ? Int32.Parse(Request.Form["encounterId"]) : 0;
            formInstance.EncounterRef = encounterId;
            if (!form.DisablePatientData)
            {
                if (encounterId == 0)
                {
                    int patientId = ParseAndInsertPatient(form.Chapters.FirstOrDefault(x => x.ThesaurusId.Equals("9356")));
                    formInstance.PatientId = patientId;
                    int eocId = InsertEpisodeOfCare(patientId, form.EpisodeOfCare, "Engine", DateTime.Now, user);
                    formInstance.EpisodeOfCareRef = eocId;
                    encounterId = InsertEncounter(eocId);
                }
                formInstance.EncounterRef = encounterId;
            }
        }
        private int ParseAndInsertPatient(FormChapter chapter)
        {
            //TO DO 
            //PatientParser patientParser = new PatientParser(customEnumBLL.GetAll().Where(x => x.Type == IdentifierKind.PatientIdentifierType.ToString()).ToList());
            PatientParser patientParser = new PatientParser(new List<Domain.Sql.Entities.Common.CustomEnum>());
            Patient patient = patientParser.ParsePatientChapter(chapter);

            return InsertPatient(patient);
        }

        protected int InsertPatient(Patient patient)
        {
            //TO DO FIX THIS FUNCTION
            Patient patientDb = patient == null || patient.Identifiers == null || patient.Identifiers.Count <= 0 ?
                patient
                :
                patientDAL.GetByIdentifier(patient.Identifiers[0]);

            if (patientDb?.Id == 0)
            {
                patientDAL.InsertOrUpdate(patientDb);
            }
        
            return patientDb.Id;
        }

        protected int InsertEpisodeOfCare(int patientId, FormEpisodeOfCare episodeOfCare, string source, DateTime startDate, UserData user)
        {
            startDate = startDate.Date;
            EpisodeOfCare eoc;
            if (episodeOfCare != null)
            {
                eoc = Mapper.Map<EpisodeOfCare>(episodeOfCare);
                eoc.Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate };
                eoc.Description = $"Generated from {source}";
                eoc.PatientId = patientId;
                eoc.DiagnosisRole = 12227;
                //eoc.OrganizationRef = userCookieData.ActiveOrganization;
                eoc.OrganizationId = 1;
            }
            else
            {
                eoc = new EpisodeOfCare()
                {
                    Description = $"Generated from {source}",
                    DiagnosisRole = 12227,
                    //OrganizationRef = userCookieData.ActiveOrganization,
                    OrganizationId = 1,
                    PatientId = patientId,
                    Status = EOCStatus.Active,
                    Period = new Domain.Sql.Entities.Common.PeriodDatetime() { Start = startDate }
                };
            }

            return episodeOfCareDAL.InsertOrUpdate(eoc, user);

        }

        protected int InsertEncounter(int episodeOfCareId)
        {
            Encounter encounterEntity = new Encounter()
            {
                Class = 12246,
                Period = new Domain.Sql.Entities.Common.PeriodDatetime()
                {
                    Start = DateTime.Now,
                    End = DateTime.Now
                },
                Status = 12218,
                EpisodeOfCareId = episodeOfCareId,
                Type = 12208,
                ServiceType = 11087
            };
            return encounterDAL.Insert(encounterEntity);
        }
        #endregion



    }
}