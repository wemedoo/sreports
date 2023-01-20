using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IDigitalGuidelineBLL
    {
        PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetAll(GuidelineFilterDataIn dataIn);
        Task<GuidelineDataOut> GetById(string id);
        Task<ResourceCreatedDTO> InsertOrUpdate(GuidelineDataIn dataIn);
        GuidelineElementDataDataOut PreviewNode(GuidelineElementDataDataIn dataIn);
        void Delete(string id, DateTime lastUpdate);
        List<GuidelineDataOut> SearchByTitle(string title);
    }
}
