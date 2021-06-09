using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface ISimilarTermService
    {
        void InsertMany(List<ThesaurusEntry> thesauruses, List<TranslationBulkInfo> bulkedTranslations, Dictionary<string, int> bulkedThesauruses);
        List<SimilarTermSearch> GetTermFiltered(List<string> terms);

    }
}
