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
        public string UserRef { get; set; }

        public string OrganizationRef { get; set; }
    }
}
