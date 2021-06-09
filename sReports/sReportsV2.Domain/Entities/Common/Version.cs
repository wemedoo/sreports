using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class Version
    {
        public string Id { get; set; }
        public VersionType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public int UserId { get; set; }

        public int OrganizationId { get; set; }

        public ThesaurusState? State { get; set; }

    }
}
