using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class CustomEnum : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CustomEnumId")]
        public int CustomEnumId { get; set; }
        public int ThesaurusEntryId { get; set; }
        public int OrganizationId { get; set; }
        public CustomEnumType Type { get; set; }
        [ForeignKey("ThesaurusEntryId")]
        public virtual ThesaurusEntry.ThesaurusEntry ThesaurusEntry { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get;set; }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusEntryId = this.ThesaurusEntryId == oldThesaurus ? newThesaurus : this.ThesaurusEntryId;
        }
    }
}
