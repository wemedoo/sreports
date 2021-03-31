using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.PatientEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IEpisodeOfCareService
    {
        bool Delete(string eocId, DateTime lastUpdate);
        bool ExistById(string id);
        EpisodeOfCareEntity GetEOCById(string id);
        List<EpisodeOfCareStatus> GetStatusHistory(string episodeOfCareId);
        List<EpisodeOfCareEntity> GetAll(EpisodeOfCareFilter filter);
        List<EpisodeOfCareEntity> GetAllByIds(List<string> ids);
        List<EpisodeOfCareEntity> GetAllByPatientId(int pageSize, int page, string id, string organizationId);
        long GetAllEntriesCountByPatientId(string id, string organizationId);
        long GetAllEntriesCount(EpisodeOfCareFilter filter);
        string InsertOrUpdate(EpisodeOfCareEntity entity);
        List<EpisodeOfCareEntity> GetByParameters(EpisodeOfCareFhirFilter episodeOfCareFilter);
        List<EpisodeOfCareEntity> GetByPatientId(string patientId);
        List<EpisodeOfCareEntity> GetAllForPatientInPeriod(string patientId , DateTime dataTimeStart, DateTime dataTimeEnd);
        EpisodeOfCareEntity GetByEncounter(string encounterId);


    }
}
