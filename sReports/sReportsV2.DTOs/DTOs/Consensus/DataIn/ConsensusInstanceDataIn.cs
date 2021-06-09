using sReportsV2.DTOs.Form.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataIn
{
    public class ConsensusInstanceDataIn
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<ConsensusQuestionDataIn> Questions { get; set; }
        public string ConsensusRef { get; set; }
        public int UserRef { get; set; }
        public bool IsOutsideUser { get; set; }
        public string IterationId { get; set; }
    }
}