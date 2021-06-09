using sReportsV2.Domain.Entities.DigitalGuideline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IDigitalGuidelineService
    {
        Task InsertOrUpdateAsync(Guideline guideline);
        Task<Guideline> GetByIdAsync(string id);

        List<Guideline> GetAll(GuidelineFilter filter);
        int GetAllCount(GuidelineFilter filter);
        bool Delete(string id, DateTime lastUpdate);


    }
}
