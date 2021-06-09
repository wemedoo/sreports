using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataOut
{
    public class ConsensusInstanceDataOut
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public List<ConsensusInstanceDataOut> Questions { get; set; }
        public string ConsensusRef { get; set; }
        public string UserRef { get; set; }
        public bool IsOutsideUser { get; set; }
        public string IterationId { get; set; }
    }
}