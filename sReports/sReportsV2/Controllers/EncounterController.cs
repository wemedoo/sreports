using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Encounter;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using sReportsV2.Common.Singleton;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using AutoMapper;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Common.Constants;

namespace sReportsV2.Controllers
{
    public class EncounterController : BaseController
    {
        private readonly IEncounterBLL encounterBLL;

        public EncounterController(IEncounterBLL encounterBLL)
        {
            this.encounterBLL = encounterBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEncounter, Module = ModuleNames.Patients)]
        public ActionResult CreateFromPatient(string patientId, string episodeOfCareId)
        {
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.ServiceType);
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterType);
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterStatus);
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterClassification);
            ViewBag.EpisodeOfCareId = episodeOfCareId;
            ViewBag.PatientId = patientId;
            return PartialView("EncounterForm");
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEncounter, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsEncounterValidate]
        public ActionResult Create(EncounterDataIn encounter)
        {
            int result = encounterBLL.InsertOrUpdate(encounter);

            return new HttpStatusCodeResult(HttpStatusCode.Created, result.ToString());
        }

        public async Task<ActionResult> ListReferralsAndForms(int encounterId, int? episodeOfCareId)
        {
            if (episodeOfCareId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, Resources.TextLanguage.EocNotFound);
            }

            var result = await encounterBLL.ListReferralsAndForms(encounterId, episodeOfCareId.GetValueOrDefault(), userCookieData).ConfigureAwait(false);
            return PartialView(result);
        }

        public async Task<ActionResult> ListForms(string condition)
        {
            var result = await encounterBLL.ListForms(condition, userCookieData).ConfigureAwait(false);
            return PartialView(result);
        }

        public ActionResult PatientTreeItems(EncounterTreeFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            List<EncounterDataOut> encounterEntities = this.encounterBLL.GetAllByEocId(dataIn.EpisodeOfCareId);

            ViewBag.EpisodeOfCareId = dataIn.EpisodeOfCareId;
            return PartialView("PatientTreeEncounterItems", encounterEntities);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public async Task<ActionResult> EditFromPatient(int encounterId)
        {
            var result = await encounterBLL.GetEncounterAndFormInstancesAndSuggestedForms(encounterId, userCookieData).ConfigureAwait(false);
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.ServiceType);
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterType);
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterStatus);
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EncounterClassification);

            return PartialView("PatientEncounter", result);
        }

        [SReportsAuthorize]
        public async Task<ActionResult> GetSuggestedForms()
        {
            var suggestedForms = await encounterBLL.GetSuggestedForms(userCookieData.SuggestedForms).ConfigureAwait(false);
            return PartialView("SuggestedForms", Mapper.Map<List<FormDataOut>>(suggestedForms));
        }

        [SReportsAuthorize]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(int id)
        {
            encounterBLL.Delete(id);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}