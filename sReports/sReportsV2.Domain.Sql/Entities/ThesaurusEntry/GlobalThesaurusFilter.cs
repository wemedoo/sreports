using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class GlobalThesaurusFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string Term { get; set; }
        public string Language { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string TermIndicator { get; set; }
    }
}
