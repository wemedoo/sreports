using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Consensus.DataOut
{
    public class ConsensusQuestionnaireDataOut
    {
        public int UserId { get; set; } 
        public bool IsOutsideUser { get; set; }
        public ConsensusDataOut Consensus { get; set; }
        public string ConsensusInstanceId { get; set; }
        public string ShowQuestionnaireType { get; set; }
        public string ViewType { get; set; }
        public string IterationId { get; set; }
        public IterationState IterationState { get; set; }
        public bool Completed { get; set; }

        public ConsensusQuestionnaireDataOut()
        {
        }

        public ConsensusQuestionnaireDataOut(ConsensusDataOut consensus)
        {
            Consensus = consensus;
        }
    }
}
