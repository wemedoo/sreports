using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class Version
    {
        public string Id { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }

        public bool IsVersionGreater(Version version) 
        {
            Ensure.IsNotNull(version, nameof(version));
            return this.Major > version.Major || this.Major == version.Major && this.Minor > version.Minor;
        }
    }
}
