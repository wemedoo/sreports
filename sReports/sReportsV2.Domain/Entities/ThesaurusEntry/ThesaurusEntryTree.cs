using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    public class ThesaurusEntryTree
    {
        public string Id { get; set; }

        public string O40MTId { get; set; }

        public string UMLSCode { get; set; }

        public string Definition { get; set; }

        public string PreferredTerm { get; set; }

        public ThesaurusEntryTree Parent { get; set; }
    }
}
