using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class DigitalGuidelineBLL : IDigitalGuidelineBLL
    {
        private readonly IDigitalGuidelineDAL digitalGuidelineDAL;
        private readonly IThesaurusDAL thesaurusDAL;

        public DigitalGuidelineBLL(IDigitalGuidelineDAL digitalGuidelineDAL, IThesaurusDAL thesaurusDAL)
        {
            this.digitalGuidelineDAL = digitalGuidelineDAL;
            this.thesaurusDAL = thesaurusDAL;
        }

        public void Delete(string id, DateTime lastUpdate)
        {
            digitalGuidelineDAL.Delete(id, lastUpdate);
        }

        public async Task<GuidelineDataOut> GetById(string id)
        {
            var data = await this.digitalGuidelineDAL.GetByIdAsync(id).ConfigureAwait(false);
            GuidelineDataOut dataOut = Mapper.Map<GuidelineDataOut>(data);

            return dataOut;
        }

        public PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> GetAll(GuidelineFilterDataIn dataIn)
        {
            GuidelineFilter filter = Mapper.Map<GuidelineFilter>(dataIn);

            PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> result = new PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn>()
            {
                Count = (int)this.digitalGuidelineDAL.GetAllCount(filter),
                Data = Mapper.Map<List<GuidelineDataOut>>(this.digitalGuidelineDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public GuidelineElementDataDataOut PreviewNode(GuidelineElementDataDataIn dataIn)
        {
            GuidelineElementDataDataOut data = Mapper.Map<GuidelineElementDataDataOut>(dataIn);
            if (dataIn.Thesaurus != null)
            {
                data.Thesaurus = Mapper.Map<ThesaurusEntryDataOut>(this.thesaurusDAL.GetById(dataIn.Thesaurus.Id));
            }

            return data;
        }

        public async Task<ResourceCreatedDTO> InsertOrUpdate(GuidelineDataIn dataIn)
        {
            Guideline guideline = await this.digitalGuidelineDAL.InsertOrUpdateAsync(Mapper.Map<Guideline>(dataIn)).ConfigureAwait(false);

            return new ResourceCreatedDTO()
            {
                Id = guideline?.Id,
                LastUpdate = guideline.LastUpdate.Value.ToString("o")
            };
        }

        public List<GuidelineDataOut> SearchByTitle(string title)
        {
            List<Guideline> result = digitalGuidelineDAL.SearchByTitle(title);
            return Mapper.Map<List<GuidelineDataOut>>(result);
        }
    }
}
