using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormInstanceService
    {
        FormInstance GetById(string formValueId);

        string InsertOrUpdate(FormInstance form);
        bool Delete(string id, DateTime lastUpdate);
        List<FormInstance> GetByFormThesaurusId(FormInstanceFilterData filterData);
        List<FormInstance> GetByEpisodeOfCare(string episodeOfCareId, int PageSize, int Page);
        List<FormInstance> GetByEpisodeOfCareId(string episodeOfCareId);
        List<FormInstance> GetAllByCovidFilter(FormInstanceCovidFilter filter);
        Task<List<FormInstance>> GetAllFieldsByCovidFilter();
        int CountEpisodeOfCare(string episodeOfCareId);
        bool ExistsFormInstance(string formValueId);
        long GetAllInstancesByThesaurusCount(FormInstanceFilterData filterData);
        IQueryable<FormInstance> GetByIds(List<string> ids);
        List<FormInstancePerDomain> GetFormInstancePerDomain();
        List<FormInstance> GetByParameters(FormInstanceFhirFilter patientFilter);
        void InsertMany(List<FormInstance> formInstances);
        bool ExistsById(string id);
        List<FormInstance> GetAllFormsByFieldIdAndValue(string id, string value);
        List<FormInstance> GetFormsByFieldThesaurusAndValue(string thesaurusId, string value);
        List<FormInstance> GetByAllByDefinitionId(string id);
        List<FormInstance> GetByDefinitionId(string id, int limit, int offset);
        int CountByDefinition(string id);


    }
}
