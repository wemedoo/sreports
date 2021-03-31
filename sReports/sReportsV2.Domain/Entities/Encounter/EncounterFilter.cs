using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Encounter
{
    public class EncounterFilter
    {
        public string EpisodeOfCareId { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

    }
}
