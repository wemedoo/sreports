using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataOut
{
    public class IterationTrackerDataOut
    {
        public string IterationId { get; set; }
        public IterationState? State { get; set; }

        public List<ConsensusInstanceTrackerDataOut> Instances { get; set; }
        public IterationTrackerDataOut() { }
        public IterationTrackerDataOut(string iterationId) 
        {
            this.IterationId = iterationId;
            this.Instances = new List<ConsensusInstanceTrackerDataOut>();
        }
    }
}