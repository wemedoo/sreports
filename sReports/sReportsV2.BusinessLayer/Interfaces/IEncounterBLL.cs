using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IEncounterBLL
    {
        int InsertOrUpdate(EncounterDataIn encounterData);
        Task<EncounterDetailsPatientTreeDataOut> ListReferralsAndForms(int encounterId, int episodeOfCareId, UserCookieData userCookieData);
        Task<List<FormDataOut>> ListForms(string condition, UserCookieData userCookieData);
        List<EncounterDataOut> GetAllByEocId(int episodeOfCareId);
        Task<EncounterDetailsPatientTreeDataOut> GetEncounterAndFormInstancesAndSuggestedForms(int encounterId, UserCookieData userCookieData);
        void Delete(int id);
        Task<List<Form>> GetSuggestedForms(List<string> suggestedFormsIds);

    }
}
