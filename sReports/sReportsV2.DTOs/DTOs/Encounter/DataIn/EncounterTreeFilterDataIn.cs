using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterTreeFilterDataIn :DataIn
    {
        public int EpisodeOfCareId { get; set; }
    }
}