using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IEpisodeOfCareDAL
    {
        bool Delete(int eocId, DateTime lastUpdate);
        EpisodeOfCare GetById(int id);
        List<EpisodeOfCare> GetAll(EpisodeOfCareFilter filter);
        long GetAllEntriesCount(EpisodeOfCareFilter filter);
        int InsertOrUpdate(EpisodeOfCare entity, UserData user);
        bool ThesaurusExist(int thesaurusId);
        void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
    }
}
