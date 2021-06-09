using AutoMapper;
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
using System.Web.Mvc;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Common;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.Domain.Entities.PatientEntities;
using Serilog;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Domain.Entities.Form;
using System.Threading.Tasks;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Enums;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;

namespace sReportsV2.Controllers
{
    public class EncounterController : BaseController
    {
        private readonly IFormInstanceDAL formInstanceService;
        private readonly IFormDAL formService;
        private readonly IEncounterDAL encounterDAL;
        private readonly IEpisodeOfCareService episodeOfCareService;

        public EncounterController(IEncounterDAL encounterDAL)
        {
            this.encounterDAL = encounterDAL;
            episodeOfCareService = new EpisodeOfCareService();
            formInstanceService = new FormInstanceDAL();
            formService = new FormDAL();
        }

        [SReportsAutorize]
        public ActionResult CreateFromPatient(string patientId, string episodeOfCareId)
        {
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.ServiceType)).ToList();
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterType)).ToList();
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterStatus)).ToList();
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterClassification)).ToList();
            ViewBag.EpisodeOfCareId = episodeOfCareId;
            ViewBag.PatientId = patientId;
            return PartialView("EncounterForm");
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [HttpPost]
        [SReportsEncounterValidate]
        public ActionResult Create(EncounterDataIn encounter)
        {
            encounter = Ensure.IsNotNull(encounter, nameof(encounter));
            var mappedEncounter = Mapper.Map<Encounter>(encounter);
            int result = encounterDAL.Insert(mappedEncounter);
            

            return new HttpStatusCodeResult(HttpStatusCode.Created, result.ToString());
        }

        public async Task<ActionResult> ListReferralsAndForms(int encounterId, string episodeOfCareId)
        {
            if (string.IsNullOrWhiteSpace(episodeOfCareId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please choose episode of care!");
            }
            Task<List<Form>> formsTask = this.formService.GetAllByOrganizationAndLanguageAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceService.GetAllByEpisodeOfCareIdAsync(episodeOfCareId);            

            await Task.WhenAll(formsTask, formInstancesTask).ConfigureAwait(false);
            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = new EncounterDataOut()
                {
                    Id = encounterId,
                    EpisodeOfCareId = episodeOfCareId
                },
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(formInstancesTask.Result),
                Forms = Mapper.Map<List<FormDataOut>>(formsTask.Result.OrderByDescending(d => userCookieData.SuggestedForms.IndexOf(d.Id)).ToList())
            };

            return PartialView(result);
        }

        public async Task<ActionResult> ListForms(string condition)
        {
            List<Form> result = await this.formService.GetAllByOrganizationAndLanguageAndNameAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, condition).ConfigureAwait(false);
            return PartialView(Mapper.Map<List<FormDataOut>>(result.OrderBy(d => userCookieData.SuggestedForms.IndexOf(d.Id)).ToList()));
        }

        public ActionResult PatientTreeItems(EncounterTreeFilterDataIn dataIn)
        {
            List<Encounter> encounterEntities= this.encounterDAL.GetAllByEocIdAsync(dataIn.EpisodeOfCareId);

            ViewBag.EpisodeOfCareId = dataIn.EpisodeOfCareId;
            return PartialView("PatientTreeEncounterItems", Mapper.Map<List<EncounterDataOut>>(encounterEntities));
        }

        [SReportsAutorize]
        public async Task<ActionResult> EditFromPatient(int encounterId)
        { 
            Encounter encounter = encounterDAL.GetById(encounterId);
            if (encounter == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var mappedEncounter = Mapper.Map<EncounterDataOut>(encounter);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceService.GetAllByEncounterAsync(encounterId);
            Task<List<Form>> suggestedFormsTask = this.formService.GetByFormIdsListAsync(userCookieData.SuggestedForms);
            await Task.WhenAll(formInstancesTask, suggestedFormsTask).ConfigureAwait(false);

            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = mappedEncounter,
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(formInstancesTask.Result),
                Forms = Mapper.Map<List<FormDataOut>>(suggestedFormsTask.Result)
            };
            ViewBag.ServiceTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.ServiceType)).ToList();
            ViewBag.EncounterType = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterType)).ToList();
            ViewBag.EncounterStatuses = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterStatus)).ToList();
            ViewBag.EncounterClassifications = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterClassification)).ToList();
            return PartialView("PatientEncounter", result);
        }

        [SReportsAutorize]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(int id, DateTime lastUpdate)
        {
            encounterDAL.Delete(id, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private void ReloadServiceTypes(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var servicesTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.ServiceType)).ToList();
                item.ServiceType = servicesTypes.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.ServiceType.Thesaurus.Id));
            }
        }

        private void ReloadTypes(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterTypes = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterType)).ToList();
                item.Type = encounterTypes.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.Type.Thesaurus.Id));
            }
        }

        private void ReloadStatuses(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterStatuses = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterStatus)).ToList();
                item.Status = encounterStatuses.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.Status.Thesaurus.Id));
            }
        }

        private void ReloadClassifications(PaginationDataOut<EncounterDataOut, DataIn> result)
        {
            foreach (var item in result.Data)
            {
                var encounterClass = SingletonDataContainer.Instance.GetEnums().Where(x => x.Type.Equals(CustomEnumType.EncounterClassification)).ToList();
                item.Class = encounterClass.FirstOrDefault(x => x.Thesaurus.Id.Equals(item.Class.Thesaurus.Id));
            }
        }

    }
}