using AutoMapper;
using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Common;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.OrganizationEntities;
using Serilog;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;

namespace sReportsV2.Controllers
{
    public class EncounterController : BaseController
    {
        private readonly IPatientService patientService;
        private readonly IEncounterService encounterService;
        private readonly IEpisodeOfCareService episodeOfCareService;
        private readonly IThesaurusEntryService thesaurusService;

        public EncounterController()
        {
            patientService = new PatientService();
            encounterService = new EncounterService();
            episodeOfCareService = new EpisodeOfCareService();
            thesaurusService = new ThesaurusEntryService();
        }

        [Authorize]
        public ActionResult Create(string eocId)
        {
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEncounterTypes();
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEncounterStatus();
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEncounterClassification();
            ViewBag.EpisodeOfCareId = eocId;
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetServiceTypes();
            return View();
        }

        [Authorize]
        public ActionResult CreateFromPatient(string episodeOfCareId)
        {
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEncounterTypes();
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEncounterStatus();
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEncounterClassification();
            ViewBag.EpisodeOfCareId = episodeOfCareId;
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetServiceTypes();
            return PartialView("Encounter");
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(EncounterDataIn encounter)
        {
            encounter = Ensure.IsNotNull(encounter, nameof(encounter));
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var servicesTypes = SingletonDataContainer.Instance.GetServiceTypes();
            encounter.ServiceType = servicesTypes.FirstOrDefault(x => x.ThesaurusId.Equals(encounter.ServiceType)).ThesaurusId;

            var mappedEncounter = Mapper.Map<EncounterEntity>(encounter);
            mappedEncounter.Period = new Period() { Start = DateTime.Now, End = null };

            try
            {
                encounterService.Insert(mappedEncounter);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [Authorize]
        public ActionResult GetAll(EpisodeOfCareFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
            return View();
        }

        [Authorize]
        public ActionResult GetAllEncounter(string eocId)
        {
            ViewBag.EocId = eocId;
            return View();
        }


        [SReportsAutorize]
        public ActionResult ReloadTable(EncounterFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<EncounterDataOut, DataIn> result = new PaginationDataOut<EncounterDataOut, DataIn>()
            {
                Count = (int)this.encounterService.GetAllEntriesCountByEocId(dataIn.EpisodeOfCareId),
                Data = Mapper.Map<List<EncounterDataOut>>(this.encounterService.GetAllByEocId(dataIn.PageSize, dataIn.Page, dataIn.EpisodeOfCareId)),
                DataIn = dataIn
            };
            ReloadServiceTypes(result);
            ReloadTypes(result);
            ReloadStatuses(result);
            ReloadClassifications(result);

            return PartialView("EncounterEntryTable", result);
        }

        [Authorize]
        public ActionResult ReloadEocTable(EpisodeOfCareFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PatientEntity patient = patientService.GetByIdentifier(new IdentifierEntity(dataIn.IdentifierType, dataIn.IdentifierValue));
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result = new PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>()
            {
                Count = (int)this.episodeOfCareService.GetAllEntriesCountByPatientId(patient.Id, userCookieData.ActiveOrganization),
                Data = Mapper.Map<List<EpisodeOfCareDataOut>>(this.episodeOfCareService.GetAllByPatientId(dataIn.PageSize, dataIn.Page, patient.Id, userCookieData.ActiveOrganization)),
                DataIn = dataIn
            };

            return PartialView("EOCEntryTable", result);
        }

        [Authorize]
        public ActionResult Edit(string encounterId)
        {
            EncounterEntity encounter = encounterService.GetById(encounterId);
            if (encounter != null)
            {
                ViewBag.EpisodeOfCareId = encounterId;

                var servicesTypes = SingletonDataContainer.Instance.GetServiceTypes();
                encounter.ServiceType = servicesTypes.FirstOrDefault(x => x.ThesaurusId.Equals(encounter.ServiceType))?.Display;
                var mappedEncounter = Mapper.Map<EncounterDataOut>(encounter);

                ViewBag.ServiceTypes = servicesTypes;
                ViewBag.EncounterType = SingletonDataContainer.Instance.GetEncounterTypes();
                ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEncounterStatus();
                ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEncounterClassification();

                return View("Create", mappedEncounter);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize]
        public ActionResult EditFromPatient(string encounterId)
        {
            EncounterEntity encounter = encounterService.GetById(encounterId);
            if (encounter != null)
            {
                ViewBag.EpisodeOfCareId = encounter.EpisodeOfCareId;

                var servicesTypes = SingletonDataContainer.Instance.GetServiceTypes();
                encounter.ServiceType = servicesTypes.FirstOrDefault(x => x.ThesaurusId.Equals(encounter.ServiceType))?.Display;
                var mappedEncounter = Mapper.Map<EncounterDataOut>(encounter);

                ViewBag.ServiceTypes = servicesTypes;
                ViewBag.EncounterType = SingletonDataContainer.Instance.GetEncounterTypes();
                ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEncounterStatus();
                ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEncounterClassification();
                ViewBag.IsFromPatient = true;
                return PartialView("Encounter", mappedEncounter);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(string id, DateTime lastUpdate)
        {
            try
            {
                encounterService.Delete(id, lastUpdate);
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

        private void ReloadServiceTypes(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var servicesTypes = SingletonDataContainer.Instance.GetServiceTypes();
                item.ServiceType.Display = servicesTypes.FirstOrDefault(x => x.ThesaurusId.Equals(item.ServiceType.Display))?.Display;
            }
        }
        private void ReloadTypes(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterTypes = SingletonDataContainer.Instance.GetEncounterTypes();
                item.Type = encounterTypes.FirstOrDefault(x => x.Thesaurus.O40MTId.Equals(item.Type.Thesaurus.O40MTId));
                item.Type.Label = item.Type.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);
            }
        }

        private void ReloadStatuses(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterStatuses = SingletonDataContainer.Instance.GetEncounterStatus();
                item.Status = encounterStatuses.FirstOrDefault(x => x.Thesaurus.O40MTId.Equals(item.Status.Thesaurus.O40MTId));
                item.Status.Label = item.Status.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);
            }
        }

        private void ReloadClassifications(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterClass = SingletonDataContainer.Instance.GetEncounterClassification();
                item.Class = encounterClass.FirstOrDefault(x => x.Thesaurus.O40MTId.Equals(item.Class.Thesaurus.O40MTId));
                item.Class.Label = item.Class.Thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);
            }
        }

    }
}