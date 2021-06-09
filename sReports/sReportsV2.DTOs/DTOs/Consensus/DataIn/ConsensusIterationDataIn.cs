using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Form.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataIn
{
    public class ConsensusIterationDataIn
    {
        public int Id { get; set; }
        public List<ConsensusQuestionDataIn> Questions { get; set; }
        public List<string> UserRefs { get; set; }
        public List<string> OutsideUserRefs { get; set; }
        public IterationState? State { get; set; }
    }
}