using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.DigitalGuidelineInstance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IDigitalGuidelineInstanceDAL
    {
        GuidelineInstance GetById(string id);
        void Insert(GuidelineInstance guidelineInstance);
        List<GuidelineInstance> GetGuidelineInstancesByEOC(int episodeOfCareId);
        Task<List<GuidelineInstance>> GetGuidelineInstancesByEOCAsync(int episodeOfCareId);
        bool Delete(string guidelineInstanceId);
        string GetNodeValue(string nodeId, string guidelineInstanceId);
        NodeState GetNodeState(string nodeId, string guidelineInstanceId);
    }
}
