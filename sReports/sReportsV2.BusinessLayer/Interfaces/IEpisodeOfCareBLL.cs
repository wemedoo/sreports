using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IEpisodeOfCareBLL
    {
        PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> GetAllFiltered(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData);
        int InsertOrUpdate(EpisodeOfCareDataIn episodeOfCareDataIn, UserCookieData userCookieData);
        EpisodeOfCareDataOut GetById(int episodeOfCareId);
        void Delete(int eocId);
    }
}
