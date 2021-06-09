using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Telecom
    {
        public int Id { get; set; }
        public string System { get; set; }
        public string Value { get; set; }
        public string Use { get; set; }

        public Telecom() { }
        public Telecom(string system, string value, string use)
        {
            this.System = system;
            this.Value = value;
            this.Use = use;
        }
    }
}
