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
        int InsertOrUpdate(Encounter encounter);
        List<Encounter> GetAllByEocId(int eocId);
        Encounter GetById(int id);
        void Delete(int encounterId);
        bool ThesaurusExist(int thesaurusId);
        void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
    }
}
