using sReportsV2.DTOs.Consensus.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Consensus.DataOut
{
    public class TrackerDataOut
    {
        public string ConsensusId { get; set; }
        public string ActiveIterationId { get; set; }
        public List<IterationTrackerDataOut> Iterations { get; set; }
    }
}
