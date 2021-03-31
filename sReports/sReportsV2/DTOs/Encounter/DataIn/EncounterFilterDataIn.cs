using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterFilterDataIn : DataIn
    {
        public string EpisodeOfCareId { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

    }
}