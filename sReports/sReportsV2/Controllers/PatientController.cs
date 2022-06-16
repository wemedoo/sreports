using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Common.DTO;

namespace sReportsV2.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IEncounterDAL encounterDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IPatientBLL patientBLL;
        public PatientController(IEncounterDAL encounterDAL, IPatientDAL patientDAL, IPatientBLL patientBLL)
        {
            this.encounterDAL = encounterDAL;
            this.patientDAL = patientDAL;
            this.patientBLL = patientBLL;
        }

        [SReportsAutorize]
        public ActionResult GetAll(PatientFilterDataIn dataIn)
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(PatientFilterDataIn dataIn)
        {
            PatientFilter filter = Mapper.Map<PatientFilter>(dataIn);
            PaginationDataOut<PatientDataOut, DataIn> result = new PaginationDataOut<PatientDataOut, DataIn>()
            {
                Count = (int)this.patientDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<PatientDataOut>>(this.patientDAL.GetAll(filter)),
                DataIn = dataIn
            };

            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            return PartialView("PatientEntryTable", result);
        }

        [SReportsAutorize]
        public ActionResult Create()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            return View("PatientEdit");
        }

        [SReportsAutorize]
        public ActionResult Edit(PatientEditDataIn patientEditDataIn)
        {
            patientEditDataIn = Ensure.IsNotNull(patientEditDataIn, nameof(patientEditDataIn));
            Patient patient = patientDAL.GetById(patientEditDataIn.PatientId);
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var mappedPatient = Mapper.Map<PatientDataOut>(patient);
            LoadEncountersForActiveEOC(mappedPatient, patientEditDataIn.EpisodeOfCareId);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.ServiceType)).ToList();
            SetActiveElementDataToViewBag(patientEditDataIn);

            return View(mappedPatient);

        }

        [SReportsAutorize]
        public ActionResult ReloadTree(PatientEditDataIn patientEditDataIn)
        {
            Patient patient = patientDAL.GetById(patientEditDataIn.PatientId);
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);
            LoadEncountersForActiveEOC(mappedPatient, patientEditDataIn.EpisodeOfCareId);

            return PartialView("PatientTree", mappedPatient);
        }

        [SReportsAutorize]
        public ActionResult EditPatientInfo(int patientId)
        {
            Patient patient = patientDAL.GetById(patientId);
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            return PartialView("PatientEdit", mappedPatient);
        }

        public ActionResult ExistIdentifier(IdentifierDataIn dataIn)
        {
            return Json(!patientDAL.ExistsPatientByIdentifier(Mapper.Map<Identifier>(dataIn)),JsonRequestBehavior.AllowGet);
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsPatientValidate]
        public ActionResult Create(PatientDataIn patient)
        {
            ResourceCreatedDTO resourceCreatedDTO;
            try
            {
                resourceCreatedDTO = patientBLL.Insert(patient);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            Response.StatusCode = 204;
            return new JsonResult()
            {
                Data = resourceCreatedDTO
            };
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(int patientId, DateTime lastUpdate)
        {
            try
            {
                patientDAL.Delete(patientId, lastUpdate);
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

        private List<IdentifierDataOut> GetPatientIdentifiersDataOut(List<Identifier> identifiers)
        {
            List<IdentifierDataOut> result = new List<IdentifierDataOut>();
            if (identifiers != null)
            {
                foreach (Identifier identifier in identifiers)
                {
                    IdentifierDataOut organizationIdentifier = new IdentifierDataOut()
                    {
                        System = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList().FirstOrDefault(x=>x.Thesaurus.Id.ToString().Equals(identifier.System)),
                        Type = identifier.Type,
                        Use = identifier.Use,
                        Value = identifier.Value
                    };

                    result.Add(organizationIdentifier);
                }
            }
            return result;
        }

        private void SetActiveElementDataToViewBag(PatientEditDataIn patientEditDataIn)
        {
            if (!string.IsNullOrWhiteSpace(patientEditDataIn.FormInstanceId) && patientEditDataIn.EncounterId != 0)
            {
                ViewBag.ActiveElementParent = patientEditDataIn.EncounterId;
                ViewBag.ActiveElement = patientEditDataIn.FormInstanceId;
                ViewBag.ActiveElementType = "forminstance";
            }
            else if (patientEditDataIn.EncounterId != 0 && patientEditDataIn.EpisodeOfCareId != 0)
            {
                ViewBag.ActiveElementParent = patientEditDataIn.EpisodeOfCareId;
                ViewBag.ActiveElement = patientEditDataIn.EncounterId;
                ViewBag.ActiveElementType = "encounter";
            }
            else if (patientEditDataIn.EpisodeOfCareId != 0)
            {
                ViewBag.ActiveElement = patientEditDataIn.EpisodeOfCareId;
                ViewBag.ActiveElementType = "episodeofcare";
            }

            ViewBag.IsPageReload = true;
        }

        private void LoadEncountersForActiveEOC(PatientDataOut patient, int episodeOfCareId)
        {
            if (episodeOfCareId != 0)
            {
                EpisodeOfCareDataOut episodeOfCareDataOut = patient.EpisodeOfCares.FirstOrDefault(x => x.Id.Equals(episodeOfCareId));
                if (episodeOfCareDataOut != null)
                {
                    episodeOfCareDataOut.Encounters = Mapper.Map<List<EncounterDataOut>>(this.encounterDAL.GetAllByEocIdAsync(episodeOfCareId));
                }
                ViewBag.EpisodeOfCareId = episodeOfCareId;
            }
        }

    }
}