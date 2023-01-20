using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IThesaurusEntryMergeBLL
    {
        void TakeBoth(int currentId);
        void MergeThesauruses(int sourceId, int targetId, List<string> valuesForMerge, UserData userData);
        int Merge(UserData userData);
    }
}
