using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class AuditLog : Entity
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Username { get; set; }
        public DateTime Time { get; set; }
        public string Json { get; set; }
    }
}
