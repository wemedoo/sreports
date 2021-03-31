using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.OrganizationEntities
{
    public class OrganizationFilter
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string PartOf { get; set; }
        public string Identifier { get; set; }
    }
}
