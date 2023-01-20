using sReportsV2.DTOs.DTOs.FormConsensus.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FormConsensus.DataIn
{
    public class ProceedConsensusDataIn
    {
        public string IterationId { get; set; }
        public string ConsensusId { get; set; }
        public string FormId { get; set; }
        public List<QuestionOccurenceConfigDTO> QuestionOccurences { get; set; }
    }
}
