using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Encounter
{
    public class EncounterFhirFilter
    {
        public int? Class { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
    }
}
