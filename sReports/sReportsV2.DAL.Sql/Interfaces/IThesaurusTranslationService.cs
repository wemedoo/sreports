using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface IThesaurusTranslationService
    {
        void InsertMany(List<ThesaurusEntry> thesauruses, Dictionary<string, int> bulkedThesauruses);
        List<TranslationBulkInfo> GetLastBulkInserted(int size);
    }
}
