using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization.DataOut;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Consensus.DataOut
{
    public class ConsensusUsersDataOut
    {
        public ConsensusOrganizationUserInfoDataOut ConsensusOrganizationUserInfoData { get; set; }
        public List<OrganizationUsersCountDataOut> OrganizationUsersCount { get; set; }

        public List<UserDataOut> Users { get; set; }
        public List<OutsideUserDataOut> OutsideUsers { get; set; }
    }
}
