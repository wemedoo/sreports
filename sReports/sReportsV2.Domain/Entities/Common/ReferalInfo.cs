using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class ReferalInfo
    {
        public string Title { get; set; }
        public List<KeyValue> ReferrableFields { get; set; }
    }
}
