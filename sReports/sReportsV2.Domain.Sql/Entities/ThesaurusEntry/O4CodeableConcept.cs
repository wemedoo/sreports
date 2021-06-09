using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class O4CodeableConcept
    {
        public int Id { get; set; }
        public CodeSystem.CodeSystem System { get; set; }
        public string Version { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Link { get; set; }
        public DateTime? VersionPublishDate { get; set; }
        public ThesaurusEntry ThesaurusEntry { get; set; }
        public int ThesaurusEntryId { get; set; }
        public DateTime? EntryDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int CodeSystemId { get; set; }

    }
}
