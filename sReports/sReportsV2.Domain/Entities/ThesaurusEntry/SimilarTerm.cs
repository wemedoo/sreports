using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    public class SimilarTerm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public SimilarTermType Source { get; set; }
        public DateTime? EntryDateTime { get; set; }
    }
}
