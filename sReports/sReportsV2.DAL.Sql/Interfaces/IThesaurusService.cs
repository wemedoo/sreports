using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface IThesaurusService
    {
        void InsertMany(List<ThesaurusEntry> thesauruses);
        Dictionary<string, int> GetLastBulkInserted(int size);
        void Insert(ThesaurusEntry thesaurus);
        string InsertOrUpdate(ThesaurusEntry thesaurus);

        ThesaurusEntry GetById(int id);
        List<string> GetAll(ThesaurusFilter filter);
        int GetAllCount();

    }
}
