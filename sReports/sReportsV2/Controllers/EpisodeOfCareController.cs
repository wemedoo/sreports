using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Exceptions;
using System;
using MongoDB.Driver.Core.Misc;

namespace sReportsV2.Controllers
{
    public class EpisodeOfCareController : BaseController
    {
        // GET: EpisodeOfCare
        private readonly IPatientService patientService;
        private readonly IEpisodeOfCareService episodeOfCareService;
        private readonly IOrganizationService organizationService;
        private readonly IUserService userService;
        public EpisodeOfCareController()
        {
            patientService = new PatientService();
            episodeOfCareService = new EpisodeOfCareService();
            organizationService = new OrganizationService();
            userService = new UserService();
        }

        [Authorize]
        public ActionResult GetAll(EpisodeOfCareFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(EpisodeOfCareFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            EpisodeOfCareFilter filter = GetFilterData(dataIn);
            
            PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result = new PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>()
            {
                Count = (int)this.episodeOfCareService.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<EpisodeOfCareDataOut>>(this.episodeOfCareService.GetAll(filter)),
                DataIn = dataIn
            };
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEpisodeOfCareTypes();
            ReloadDiagnosisRoles(result);

            return PartialView("EOCEntryTable", result);
        }

        [Authorize]
        public ActionResult Create(IdentifierDataIn dataIn)
        {
            PatientEntity patient = patientService.GetByIdentifier(Mapper.Map<IdentifierEntity>(dataIn));
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);

            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetDiagnosisRoles();
            ViewBag.Patient = mappedPatient;
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEpisodeOfCareTypes();
            return View("Edit");
        }

        [Authorize]
        public ActionResult CreateFromPatient(string patientId)
        {
            PatientEntity patient = patientService.GetById(patientId);
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);

            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetDiagnosisRoles();
            ViewBag.Patient = mappedPatient;
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEpisodeOfCareTypes();
            return PartialView("EpisodeOfCareInformation");
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(EpisodeOfCareDataIn episodeOfCare)
        {
            episodeOfCare = Ensure.IsNotNull(episodeOfCare, nameof(episodeOfCare));

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EpisodeOfCareEntity mapperEntity = Mapper.Map<EpisodeOfCareEntity>(episodeOfCare);
            if (episodeOfCare.Id != null)
            {
                mapperEntity.ListHistoryStatus = episodeOfCareService.GetStatusHistory(episodeOfCare.Id);
            }
            mapperEntity.UpdateHistory();
            mapperEntity.OrganizationRef = userCookieData.GetActiveOrganizationData().Id;

            try
            {
                episodeOfCareService.InsertOrUpdate(mapperEntity);
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
        public ActionResult EditFromPatient(string episodeOfCareId)
        {
            EpisodeOfCareEntity eoc = episodeOfCareService.GetEOCById(episodeOfCareId);
            if (eoc != null)
            {
                var episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(eoc);
                PatientEntity patient = patientService.GetById(eoc.PatientId);

                if (patient == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                ViewBag.Patient = Mapper.Map<PatientDataOut>(patient);
                ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetDiagnosisRoles();
                ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEpisodeOfCareTypes();
                return PartialView("EpisodeOfCareInformation" , episodeOfCareDataOut);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [Authorize]
        public ActionResult Edit(string episodeOfCareId)
        {
            EpisodeOfCareEntity eoc = episodeOfCareService.GetEOCById(episodeOfCareId);
            if (eoc != null)
            {
                var episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(eoc);
                PatientEntity patient = patientService.GetById(eoc.PatientId);

                if (patient == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetDiagnosisRoles();
                ViewBag.Patient = Mapper.Map<PatientDataOut>(patient);
                ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEpisodeOfCareTypes();
                return View(episodeOfCareDataOut);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }


        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteEOC(string eocId, DateTime lastUpdate)
        {
            try
            {
                episodeOfCareService.Delete(eocId, lastUpdate);
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

        private EpisodeOfCareFilter GetFilterData(EpisodeOfCareFilterDataIn dataIn)
        {
            EpisodeOfCareFilter result = Mapper.Map<EpisodeOfCareFilter>(dataIn);
            if(!string.IsNullOrEmpty(dataIn.IdentifierType) && !string.IsNullOrEmpty(dataIn.IdentifierValue))
            {
                result.FilterByIdentifier = true;
                result.PatientId = this.patientService.GetByIdentifier(new IdentifierEntity(dataIn.IdentifierType, dataIn.IdentifierValue))?.Id;
            }

            result.OrganizationId = userCookieData.ActiveOrganization;
            return result;
        }

        private void ReloadDiagnosisRoles(PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result)
        {
            foreach (var item in result.Data)
            {
                var diagnosisRoles = SingletonDataContainer.Instance.GetDiagnosisRoles();
                item.DiagnosisRole = diagnosisRoles.FirstOrDefault(x => x.Thesaurus.O40MTId.Equals(item.DiagnosisRole.Thesaurus.O40MTId));
                item.DiagnosisRole.Label = item.DiagnosisRole?.Thesaurus?.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage);
            }
        }
    }
}