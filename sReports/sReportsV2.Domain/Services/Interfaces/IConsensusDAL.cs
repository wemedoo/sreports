using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IConsensusDAL
    {
        void Insert(Consensus consensus);
        Consensus GetByFormId(string formId);
        Consensus GetById(string id);
        ConsensusIteration GetLastIteration(string id);

        bool IsLastIterationFinished(string consensusId);
        bool CanStartConsensusFindingProcess(string consensusId);
        IterationState GetLastIterationState(string consensusId);
    }
}
