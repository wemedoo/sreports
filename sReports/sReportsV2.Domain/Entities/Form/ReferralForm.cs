using sReportsV2.Domain.Entities.FieldEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class ReferralForm
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public int UserId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int OrganizationId { get; set; }
        public List<Field> Fields { get; set; }
    }
}
