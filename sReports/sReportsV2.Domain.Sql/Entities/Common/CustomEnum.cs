using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.EntitiesBase;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class CustomEnum : Entity
    {
        public int Id { get; set; }
        public int ThesaurusEntryId { get; set; }
        public int OrganizationId { get; set; }
        public CustomEnumType Type { get; set; }
        public virtual ThesaurusEntry.ThesaurusEntry ThesaurusEntry { get; set; }
        public virtual Organization Organization { get;set; }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusEntryId = this.ThesaurusEntryId == oldThesaurus ? newThesaurus : this.ThesaurusEntryId;
        }
    }
}
