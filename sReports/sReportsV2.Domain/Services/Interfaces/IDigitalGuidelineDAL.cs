using sReportsV2.Domain.Entities.DigitalGuideline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IDigitalGuidelineDAL
    {
        Task<Guideline> InsertOrUpdateAsync(Guideline guideline);
        Task<Guideline> GetByIdAsync(string id);
        Guideline GetById(string id);
        List<Guideline> GetAll();
        List<Guideline> GetAll(GuidelineFilter filter);
        List<Guideline> SearchByTitle(string title);
        int GetAllCount(GuidelineFilter filter);
        bool Delete(string id, DateTime lastUpdate);
        Tuple<List<GuidelineEdgeElementData>, List<GuidelineEdgeElementData>> GetEdges(string nodeId, string guidelineId);

    }
}
