using AutoMapper;
using Serilog;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
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
using sReportsV2.Domain.Exceptions;
using System;
using MongoDB.Driver.Core.Misc;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.Patient;

namespace sReportsV2.Controllers
{
    public class EpisodeOfCareController : BaseController
    {
        // GET: EpisodeOfCare
        private readonly IPatientDAL patientDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        public EpisodeOfCareController(IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL)
        {
            this.patientDAL = patientDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
        }

        [SReportsAutorize]
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
                Count = (int)this.episodeOfCareDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<EpisodeOfCareDataOut>>(this.episodeOfCareDAL.GetAll(filter)),
                DataIn = dataIn
            };
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.PatientIdentifierType)).ToList();
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();
            ReloadDiagnosisRoles(result);

            return PartialView("EOCEntryTable", result);
        }

        [SReportsAutorize]
        public ActionResult Create(IdentifierDataIn dataIn)
        {
            Patient patient = patientDAL.GetByIdentifier(Mapper.Map<Identifier>(dataIn));
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);

            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
            ViewBag.Patient = mappedPatient;
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();
            return View("Edit");
        }

        [SReportsAutorize]
        public ActionResult CreateFromPatient(int patientId)
        {
            Patient patient = patientDAL.GetById(patientId);
            if (patient == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var mappedPatient = Mapper.Map<PatientDataOut>(patient);

            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
            ViewBag.Patient = mappedPatient;
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();
            return PartialView("PatientEpisodeOfCareForm");
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsEpisodeOfCareValidate]
        public ActionResult Create(EpisodeOfCareDataIn episodeOfCare)
        {
            episodeOfCare = Ensure.IsNotNull(episodeOfCare, nameof(episodeOfCare));
            EpisodeOfCare mapperEntity = Mapper.Map<EpisodeOfCare>(episodeOfCare);
            mapperEntity.OrganizationId = userCookieData.GetActiveOrganizationData().Id;
            UserData userData = Mapper.Map<UserData>(userCookieData);

            try
            {
                episodeOfCareDAL.InsertOrUpdate(mapperEntity, userData);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Created, mapperEntity.Id.ToString());
        }

        [SReportsAutorize]
        public ActionResult EditFromPatient(int episodeOfCareId)
        {
            EpisodeOfCare eoc = episodeOfCareDAL.GetById(episodeOfCareId);
            if (eoc != null)
            {
                var episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(eoc);
                Patient patient = patientDAL.GetById(eoc.PatientId);

                if (patient == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                ViewBag.Patient = Mapper.Map<PatientDataOut>(patient);
                ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
                ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EpisodeOfCareType)).ToList();
                return PartialView("PatientEpisodeOfCareForm", episodeOfCareDataOut);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [SReportsAutorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteEOC(int eocId, DateTime lastUpdate)
        {
            episodeOfCareDAL.Delete(eocId, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private EpisodeOfCareFilter GetFilterData(EpisodeOfCareFilterDataIn dataIn)
        {
            EpisodeOfCareFilter result = Mapper.Map<EpisodeOfCareFilter>(dataIn);
            if(!string.IsNullOrEmpty(dataIn.IdentifierType) && !string.IsNullOrEmpty(dataIn.IdentifierValue))
            {
                result.FilterByIdentifier = true;
                result.PatientId = this.patientDAL.GetByIdentifier(new Identifier(dataIn.IdentifierType, dataIn.IdentifierValue)) != null ? this.patientDAL.GetByIdentifier(new Identifier(dataIn.IdentifierType, dataIn.IdentifierValue)).Id : 0;
            }

            result.OrganizationId = userCookieData.ActiveOrganization;
            return result;
        }

        private void ReloadDiagnosisRoles(PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result)
        {
            foreach (var item in result.Data)
            {
                var diagnosisRoles = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.DiagnosisRole)).ToList();
                item.DiagnosisRole = diagnosisRoles.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.DiagnosisRole.Thesaurus.Id));
            }
        }
    }
}