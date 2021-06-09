using sReportsV2.Domain.Sql.Entities.Encounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IEncounterDAL
    {
        int Insert(Encounter encounter);
        List<Encounter> GetAllByEocIdAsync(int eocId);
        Encounter GetById(int id);
        void Delete(int encounterId, DateTime lastUpdate);
        bool ThesaurusExist(int thesaurusId);
        void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
    }
}
