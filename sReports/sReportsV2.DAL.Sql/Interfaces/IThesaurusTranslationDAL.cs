using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IThesaurusTranslationDAL
    {
        void InsertMany(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses);
        List<TranslationBulkInfo> GetLastBulkInserted(int size);
    }
}
