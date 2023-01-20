using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Pagination;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;

namespace sReportsV2.Controllers
{
    public class EpisodeOfCareController : BaseController
    {
        private readonly IEpisodeOfCareBLL episodeOfCareBLL;
        private readonly IPatientBLL patientBLL;

        public EpisodeOfCareController(IEpisodeOfCareBLL episodeOfCareBLL, IPatientBLL patientBLL)
        {
            this.episodeOfCareBLL = episodeOfCareBLL;
            this.patientBLL = patientBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult GetAll(EpisodeOfCareFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(EpisodeOfCareFilterDataIn dataIn)
        {
            PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result = episodeOfCareBLL.GetAllFiltered(dataIn, userCookieData);

            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.PatientIdentifierType);
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EpisodeOfCareType);
            ReloadDiagnosisRoles(result);

            return PartialView("EOCEntryTable", result);
        }

        [SReportsAuthorize]
        public ActionResult Create(IdentifierDataIn dataIn)
        {
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.DiagnosisRole);
            ViewBag.Patient = patientBLL.GetByIdentifier(Mapper.Map<Identifier>(dataIn));
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EpisodeOfCareType);

            return View("Edit");
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEpisodeOfCare, Module = ModuleNames.Patients)]
        public ActionResult CreateFromPatient(int patientId)
        {
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.DiagnosisRole);
            ViewBag.Patient = patientBLL.GetById(patientId);
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EpisodeOfCareType);

            return PartialView("PatientEpisodeOfCareForm");
        }

        [SReportsAuthorize(Permission = PermissionNames.AddEpisodeOfCare, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsEpisodeOfCareValidate]
        public ActionResult Create(EpisodeOfCareDataIn episodeOfCare)
        {
            int episodeOfCareId = episodeOfCareBLL.InsertOrUpdate(episodeOfCare, userCookieData);
            return new HttpStatusCodeResult(HttpStatusCode.Created, episodeOfCareId.ToString());
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult EditFromPatient(int episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = episodeOfCareBLL.GetById(episodeOfCareId);

            ViewBag.Patient = patientBLL.GetById(episodeOfCareDataOut.PatientId);
            ViewBag.DiagnosisRoles = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.DiagnosisRole);
            ViewBag.EpisodeOfCareTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.EpisodeOfCareType);

            return PartialView("PatientEpisodeOfCareForm", episodeOfCareDataOut);
        }

        [SReportsAuthorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteEOC(int eocId)
        {
            episodeOfCareBLL.Delete(eocId);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void ReloadDiagnosisRoles(PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result)
        {
            foreach (var item in result.Data)
            {
                var diagnosisRoles = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.DiagnosisRole);
                item.DiagnosisRole = diagnosisRoles.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.DiagnosisRole.Thesaurus.Id));
            }
        }
    }
}