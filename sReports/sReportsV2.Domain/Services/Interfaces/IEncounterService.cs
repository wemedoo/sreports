using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IEncounterService
    {
        string Insert(EncounterEntity encounter);
        List<ServiceType> GetAllServiceTypes();
        long GetAllEntriesCount();
        long GetAllEntriesCountByEocId(string id);
        List<EncounterEntity> GetAll(int pageSize, int page);
        List<EncounterEntity> GetAllByEocId(int pageSize, int page, string eocId);
        EncounterEntity GetById(string id);
        List<EncounterEntity> GetByIds(List<string> id);
        List<EncounterEntity> GetByObjectId(string id);
        List<EncounterEntity> GetByParameters(EncounterFhirFilter encounterFilter);
        bool Delete(string encounterId, DateTime lastUpdate);
        bool ExistEncounter(string encounterId);
        EncounterEntity GetLatestByEpisodeOFCare(string episodeOfCareId);
    }
}
