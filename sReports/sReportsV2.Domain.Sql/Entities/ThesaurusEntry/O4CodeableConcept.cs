using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class O4CodeableConcept
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("O4CodeableConceptId")]
        public int O4CodeableConceptId { get; set; }
        public CodeSystem.CodeSystem System { get; set; }
        public string Version { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Link { get; set; }
        public DateTime? VersionPublishDate { get; set; }
        public DateTime? EntryDateTime { get; set; }
        public bool IsDeleted { get; set; }
        [Column("CodeSystemId")]
        public int CodeSystemId { get; set; }
        [Column("ThesaurusEntryId")]
        public int? ThesaurusEntryId { get; set; }

        public void Copy(O4CodeableConcept code)
        {
            Version = code.Version;
            VersionPublishDate = code.VersionPublishDate;
            Code = code.Code;
            Value = code.Value;
            Link = code.Link;
            CodeSystemId = code.CodeSystemId;
        }

    }
}
