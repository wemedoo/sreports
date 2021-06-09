﻿using sReportsV2.Domain.Entities.Consensus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IConsensusInstanceService
    {
        void InsertOrUpdate(ConsensusInstance consensusInstance);
        ConsensusInstance GetByConsensusAndUserAndIteration(string consensusId, int userId, string iterationId);
        ConsensusInstance GetById(string id);
        List<ConsensusInstance> GetAllByConsensusId(string consensusId);

    }
}
