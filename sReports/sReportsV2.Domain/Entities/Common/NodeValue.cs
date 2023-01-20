using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class NodeValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public NodeState State { get; set; }
    }
}
