using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class EncounterBLL : IEncounterBLL
    {
        private readonly IEncounterDAL encounterDAL;
        private readonly IFormInstanceDAL formInstanceDAL;
        private readonly IFormDAL formDAL;
        public EncounterBLL(IEncounterDAL encounterDAL)
        {
            this.encounterDAL = encounterDAL;
            formInstanceDAL = new FormInstanceDAL();
            formDAL = new FormDAL();
        }

        public void Delete(int id)
        {
            encounterDAL.Delete(id);
        }

        public async Task<EncounterDetailsPatientTreeDataOut> GetEncounterAndFormInstancesAndSuggestedForms(int encounterId, UserCookieData userCookieData)
        {
            Encounter encounter = encounterDAL.GetById(encounterId);
            if (encounter == null)
            {
                throw new NullReferenceException("Please choose episode of care!");
            }

            var encounterDataOut = Mapper.Map<EncounterDataOut>(encounter);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceDAL.GetAllByEncounterAsync(encounterId, userCookieData.ActiveOrganization);
            Task<List<Form>> suggestedFormsTask = this.formDAL.GetByFormIdsListAsync(userCookieData.SuggestedForms);
            await Task.WhenAll(formInstancesTask, suggestedFormsTask).ConfigureAwait(false);

            EncounterDetailsPatientTreeDataOut result = new EncounterDetailsPatientTreeDataOut()
            {
                Encounter = encounterDataOut,
                FormInstances = Mapper.Map<List<FormInstanceDataOut>>(formInstancesTask.Result),
                Forms = Mapper.Map<List<FormDataOut>>(suggestedFormsTask.Result)
            };

            return result;
        }

        public List<EncounterDataOut> GetAllByEocId(int episodeOfCareId)
        {
            return Mapper.Map<List<EncounterDataOut>>(encounterDAL.GetAllByEocId(episodeOfCareId));
        }

        public int InsertOrUpdate(EncounterDataIn encounterData)
        {
            encounterData = Ensure.IsNotNull(encounterData, nameof(encounterData));
            var encounter = Mapper.Map<Encounter>(encounterData);

            return encounterDAL.InsertOrUpdate(encounter);
        }

        public async Task<List<FormDataOut>> ListForms(string condition, UserCookieData userCookieData)
        {
            List<Form> result = await this.formDAL.GetAllByOrganizationAndLanguageAndNameAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, condition).ConfigureAwait(false);

            return Mapper.Map<List<FormDataOut>>(result.OrderBy(d => userCookieData.SuggestedForms.IndexOf(d.Id)).ToList());
        }

        public async Task<EncounterDetailsPatientTreeDataOut> ListReferralsAndForms(int encounterId, int episodeOfCareId, UserCookieData userCookieData)
        {
            Task<List<Form>> formsTask = this.formDAL.GetAllByOrganizationAndLanguageAsync(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
            Task<List<FormInstance>> formInstancesTask = this.formInstanceDAL.GetAllByEpisodeOfCareIdAsync(episodeOfCareId, userCookieData.ActiveOrganization);

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

            return result;
        }

        public Task<List<Form>> GetSuggestedForms(List<string> suggestedFormsIds)
        {
            return this.formDAL.GetByFormIdsListAsync(suggestedFormsIds);
        }
    }
}
