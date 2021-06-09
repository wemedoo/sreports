using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class SimilarTermSearch
    {
        public string Name { get; set; }
        public int ThesaurusEntryTranslationId { get; set; }
    }
}
