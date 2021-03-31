using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IPatientService patientService;

        public PatientController()
        {
            patientService = new PatientService();
        }

        [Authorize]
        public ActionResult GetAll(PatientFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(PatientFilterDataIn dataIn)
        {
            PatientFilter filter = Mapper.Map<PatientFilter>(dataIn);
            PaginationDataOut<PatientDataOut, DataIn> result = new PaginationDataOut<PatientDataOut, DataIn>()
            {
                Count = (int)this.patientService.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<PatientDataOut>>(this.patientService.GetAll(filter)),
                DataIn = dataIn
            };

            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
            return PartialView("PatientEntryTable", result);
        }

        [Authorize]
        public ActionResult ReloadTree(string patientId)
        {
            PatientEntity patient = patientService.GetExpandedPatientById(patientId, userCookieData.ActiveOrganization);
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);
           
            return PartialView("PatientTree", mappedPatient);
        }

        [Authorize]
        public ActionResult CreateDocument(string patientId, string encounterId, string episodeOfCareId)
        {
            ViewBag.EncounterId = encounterId;
            ViewBag.EpisodeOfCareId = episodeOfCareId;
            PatientEntity patient = patientService.GetExpandedPatientById(patientId, userCookieData.ActiveOrganization);
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);
            return PartialView("CreateDocument", mappedPatient);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
            return View("PatientEdit");
        }

        [Authorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(PatientDataIn patient)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Log.Error(string.Join(", ", allErrors));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mappedPatient = Mapper.Map<PatientEntity>(patient);

            try
            {
                patientService.Insert(mappedPatient);
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
        public ActionResult Edit(string patientId)
        {
            PatientEntity patient = patientService.GetExpandedPatientById(patientId, userCookieData.ActiveOrganization);
            if (patient != null)
            {
                var mappedPatient = Mapper.Map<PatientDataOut>(patient);
                mappedPatient.Identifiers = this.GetPatientIdentifiersDataOut(patient.Identifiers);
                ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
                ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetServiceTypes();

                return View(mappedPatient);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);          
        }

        [Authorize]
        public ActionResult EditPatientInfo(string patientId)
        {
            PatientEntity patient = patientService.GetExpandedPatientById(patientId, userCookieData.ActiveOrganization);
            if (patient != null)
            {
                var mappedPatient = Mapper.Map<PatientDataOut>(patient);
                mappedPatient.Identifiers = this.GetPatientIdentifiersDataOut(patient.Identifiers);
                ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetIdentifierTypes();
               

                return PartialView("PatientEdit" , mappedPatient);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        public ActionResult ExistIdentifier(IdentifierDataIn dataIn)
        {
            return Json(!patientService.ExistsPatientByIdentifier(Mapper.Map<IdentifierEntity>(dataIn)),JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SReportsAuditLog]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(string patientId, DateTime lastUpdate)
        {
            try
            {
                patientService.Delete(patientId, lastUpdate);
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

        private List<IdentifierDataOut> GetPatientIdentifiersDataOut(List<IdentifierEntity> identifiers)
        {
            List<IdentifierDataOut> result = new List<IdentifierDataOut>();
            if (identifiers != null)
            {
                foreach (IdentifierEntity identifier in identifiers)
                {
                    IdentifierDataOut organizationIdentifier = new IdentifierDataOut()
                    {
                        System = new IdentifierTypeDataOut()
                        {
                            O4MtId = identifier.System,
                            Name = SingletonDataContainer.Instance.GetIdentifierTypes().FirstOrDefault(x => x.O4MtId.Equals(identifier.System))?.Name
                        },
                        Type = identifier.Type,
                        Use = identifier.Use,
                        Value = identifier.Value
                    };

                    result.Add(organizationIdentifier);
                }
            }
            return result;
        }
    }
}