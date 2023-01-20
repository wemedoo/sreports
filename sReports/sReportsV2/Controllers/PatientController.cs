using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using System.Collections.Generic;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.CustomEnum.DataOut;

namespace sReportsV2.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IPatientBLL patientBLL;
        private readonly IEncounterBLL encounterBLL;
        public PatientController(IPatientBLL patientBLL, IEncounterBLL encounterBLL)
        {
            this.patientBLL = patientBLL;
            this.encounterBLL = encounterBLL;
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult GetAll(PatientFilterDataIn dataIn)
        {
            SetCountryNameIfFilterByCountryIsIncluded(dataIn);
            ViewBag.FilterData = dataIn;
            SetIdentifierTypesToViewBag();
            return View();
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult ReloadTable(PatientFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            dataIn.OrganizationId = userCookieData.ActiveOrganization;
            PopulateGenders(dataIn);
            PopulateActivity(dataIn);
            PaginationDataOut<PatientDataOut, DataIn> result = patientBLL.GetAllFiltered(dataIn);
            SetIdentifierTypesToViewBag();

            return PartialView("PatientEntryTable", result);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Patients)]
        public ActionResult Create()
        {
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(false);
            return View("PatientEdit");
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult Edit(PatientEditDataIn patientEditDataIn)
        {
            patientEditDataIn = Ensure.IsNotNull(patientEditDataIn, nameof(patientEditDataIn));

            var patient = patientBLL.GetById(patientEditDataIn.PatientId);
            SetLoadEncountersForActiveEOCToViewBag(patient, patientEditDataIn.EpisodeOfCareId);
            SetIdentifierTypesToViewBag();
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.ServiceType);
            SetActiveElementDataToViewBag(patientEditDataIn);

            return View(patient);

        }

        [SReportsAuthorize]
        public ActionResult ReloadTree(PatientEditDataIn patientEditDataIn)
        {
            patientEditDataIn = Ensure.IsNotNull(patientEditDataIn, nameof(patientEditDataIn));
            var patient = patientBLL.GetById(patientEditDataIn.PatientId);
            SetLoadEncountersForActiveEOCToViewBag(patient, patientEditDataIn.EpisodeOfCareId);

            return PartialView("PatientTree", patient);
        }

        [SReportsAuthorize(Permission = PermissionNames.View, Module = ModuleNames.Patients)]
        public ActionResult EditPatientInfo(int patientId)
        {
            var patient = patientBLL.GetById(patientId);
            SetPatientInfoPropertiesToViewBag();
            SetReadOnlyAndDisabledViewBag(!ViewBag.UserCookieData.UserHasPermission(PermissionNames.CreateUpdate, ModuleNames.Patients));

            return PartialView("PatientEdit", patient);
        }

        public ActionResult ExistIdentifier(IdentifierDataIn dataIn)
        {
            return Json(!patientBLL.ExistsPatientByIdentifier(Mapper.Map<Identifier>(dataIn)),JsonRequestBehavior.AllowGet);
        }

        [SReportsAuthorize(Permission = PermissionNames.CreateUpdate, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsPatientValidate]
        public ActionResult Create(PatientDataIn patient)
        {
            ResourceCreatedDTO resourceCreatedDTO = patientBLL.InsertOrUpdate(patient, Mapper.Map<UserData>(userCookieData));

            Response.StatusCode = 201;
            return new JsonResult()
            {
                Data = resourceCreatedDTO
            };
        }

        [SReportsAuthorize(Permission = PermissionNames.Delete, Module = ModuleNames.Patients)]
        [SReportsAuditLog]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(int patientId)
        {
            patientBLL.Delete(patientId);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult GetByName(string searchValue)
        {
            searchValue = searchValue.RemoveDiacritics(); // Normalization

            List<PatientDataOut> patients = this.patientBLL.GetPatientsByName(searchValue);
            return PartialView("~/Views/Patient/GetByName.cshtml", patients.OrderBy(x => x.Name).ToList()); 
        }

        [HttpGet]
        public ActionResult ReloadPatients(PatientSearchFilter patientSearchFilter)
        {
            List<PatientDataOut> result = patientBLL.GetPatientsByFirstAndLastName(patientSearchFilter);
            return PartialView("~/Views/Patient/PatientAutocomplete.cshtml", result);
        }

        public ActionResult GetAutoCompleteCustomEnumData(AutocompleteDataIn dataIn, CustomEnumType customEnumType)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = FilterCustomEnumByName(dataIn.Term, customEnumType);
            var enumDataOuts = filtered
                .OrderBy(x => x.text).Skip(dataIn.Page * 15).Take(15)
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList();

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = enumDataOuts
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomEnum(int customEnumId)
        {
            CustomEnumDataOut customEnum = SingletonDataContainer.Instance.GetCustomEnum(customEnumId);
            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage as string;
            var result = new AutocompleteDataOut()
            {
                id = customEnum.Id.ToString(),
                text = customEnum.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void SetCountryNameIfFilterByCountryIsIncluded(PatientFilterDataIn dataIn)
        {
            if (dataIn != null)
            {
                string countryName = string.Empty;
                if (dataIn.CountryId.HasValue)
                {
                    countryName = SingletonDataContainer.Instance.GetCustomEnumPreferredTerm(dataIn.CountryId.Value);
                }
                dataIn.CountryName = countryName;
            }
        }

        private void PopulateGenders(PatientFilterDataIn dataIn)
        {
            foreach (var state in (Gender[])Enum.GetValues(typeof(Gender)))
                dataIn.Genders.Add(Resources.TextLanguage.ResourceManager.GetString(state.ToString()));
        }

        private void PopulateActivity(PatientFilterDataIn dataIn)
        {
            dataIn.Activity.Add(Resources.TextLanguage.Active);
            dataIn.Activity.Add(Resources.TextLanguage.Inactive);
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

        private void SetLoadEncountersForActiveEOCToViewBag(PatientDataOut patient, int episodeOfCareId)
        {
            if (episodeOfCareId != 0)
            {
                EpisodeOfCareDataOut episodeOfCareDataOut = patient.EpisodeOfCares.FirstOrDefault(x => x.Id.Equals(episodeOfCareId));
                if (episodeOfCareDataOut != null)
                {
                    episodeOfCareDataOut.Encounters = this.encounterBLL.GetAllByEocId(episodeOfCareId);
                }
                ViewBag.EpisodeOfCareId = episodeOfCareId;
            }
        }

        private void SetPatientInfoPropertiesToViewBag()
        {
            SetIdentifierTypesToViewBag();
            ViewBag.Citizenships = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.Citizenship);
            ViewBag.AddressTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.AddressType);
        }

        private void SetIdentifierTypesToViewBag()
        {
            ViewBag.IdentifierTypes = SingletonDataContainer.Instance.GetEnumsByType(CustomEnumType.PatientIdentifierType);
        }

        private IEnumerable<AutocompleteDataOut> FilterCustomEnumByName(string term, CustomEnumType customEnumType)
        {
            string activeLanguage = ViewBag.UserCookieData.ActiveLanguage as string;
            return SingletonDataContainer.Instance.GetEnumsByType(customEnumType).Where(customEnum => FilterCustomEnumByName(customEnum, activeLanguage, term)).Select(e => new AutocompleteDataOut()
            {
                id = e.Id.ToString(),
                text = e.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage)
            });
        }

        private bool FilterCustomEnumByName(CustomEnumDataOut customEnum, string activeLanguage, string term)
        {
            string preferredTerm = customEnum.Thesaurus.GetPreferredTermByTranslationOrDefault(activeLanguage);
            return !string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(preferredTerm) && preferredTerm.ToLower().Contains(term.ToLower());
        }
    }
}