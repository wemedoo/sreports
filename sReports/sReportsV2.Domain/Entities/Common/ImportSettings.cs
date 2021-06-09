using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class ImportSettings :Entity
    {
        public string Type { get; set; }
        public int Version { get; set; }
    }
}
