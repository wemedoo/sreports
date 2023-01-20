using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Consensus.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class ConsensusDataIn
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string FormRef { get; set; }
        public List<ConsensusQuestionDataIn> Questions { get; set; }
        public List<string> UserRefs { get; set; }
        public List<string> OutsideUserRefs { get; set; }
        public ConsensusFindingState? State { get; set; }
        public List<ConsensusIterationDataIn> Iterations { get; set; }


    }
}