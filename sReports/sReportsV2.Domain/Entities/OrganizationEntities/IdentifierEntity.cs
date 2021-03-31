using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.OrganizationEntities
{
    public class IdentifierEntity
    {
        public string System { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string Use { get; set; }

        public IdentifierEntity() { }

        public IdentifierEntity(string system, string value)
        {
            this.System = system;
            this.Value = value;
        }

        public IdentifierEntity(string system, string value, string type, string use)
        {
            this.System = system;
            this.Value = value;
            this.Type = type;
            this.Use = use;
        }
    }
}
