using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.EpisodeOfCareEntities
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class EpisodeOfCareWorkflow
    {
        public string DiagnosisCondition { get; set; }
        public int DiagnosisRole { get; set; }
        public int User { get; set; }
        public DateTime Submited { get; set; }
        public EOCStatus Status { get; set; }

    }
}
