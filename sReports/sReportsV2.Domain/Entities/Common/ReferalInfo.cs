using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class ReferalInfo
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public int UserId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int OrganizationId { get; set; }
        public List<KeyValue> ReferrableFields { get; set; }
    }
}
