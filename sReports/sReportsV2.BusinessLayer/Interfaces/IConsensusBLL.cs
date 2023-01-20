using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IConsensusBLL
    {
        ConsensusDataOut GetById(string id);
        void RemindUser(List<int> usersIds, string consensusId, bool isOutsideUsers, string iterationId);
        void StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn);
        ResourceCreatedDTO StartNewIteration(string consensusId, string formId, int creatorId);
        ResourceCreatedDTO TerminateCurrentIteration(string consensusId);
        ConsensusDataOut ProceedIteration(ProceedConsensusDataIn proceedConsensusDataIn);
        void AddQuestion(ConsensusQuestionDataIn consensusQuestionDataIn);
        ResourceCreatedDTO SubmitConsensusInstance(ConsensusInstanceDataIn consensusInstance);
        bool CanSubmitConsensusInstance(ConsensusInstanceDataIn consensusInstance);
        TrackerDataOut GetTrackerData(string consensusId);
        List<UserDataOut> SaveUsers(List<int> usersIds, string consensusId);
        ConsensusUsersDataOut GetConsensusUsers(string consensusId, int activeOrganization);
    }
}
