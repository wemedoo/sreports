using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class EpisodeOfCareBLL : IEpisodeOfCareBLL
    {
        private readonly IPatientDAL patientDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;

        public EpisodeOfCareBLL(IEpisodeOfCareDAL episodeOfCareDAL, IPatientDAL patientDAL)
        {
            this.patientDAL = patientDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
        }

        public void Delete(int eocId)
        {
            episodeOfCareDAL.Delete(eocId);
        }

        public PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> GetAllFiltered(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            EpisodeOfCareFilter filter = GetFilterData(dataIn, userCookieData);

            PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn> result = new PaginationDataOut<EpisodeOfCareDataOut, EpisodeOfCareFilterDataIn>()
            {
                Count = (int)this.episodeOfCareDAL.GetAllEntriesCount(filter),
                Data = Mapper.Map<List<EpisodeOfCareDataOut>>(this.episodeOfCareDAL.GetAll(filter)),
                DataIn = dataIn
            };

            return result;
        }

        public EpisodeOfCareDataOut GetById(int episodeOfCareId)
        {
            EpisodeOfCare eoc = episodeOfCareDAL.GetById(episodeOfCareId);
            if (eoc == null) throw new NullReferenceException();

            return Mapper.Map<EpisodeOfCareDataOut>(eoc);
        }

        public int InsertOrUpdate(EpisodeOfCareDataIn episodeOfCareDataIn, UserCookieData userCookieData)
        {
            episodeOfCareDataIn = Ensure.IsNotNull(episodeOfCareDataIn, nameof(episodeOfCareDataIn));

            EpisodeOfCare episodeOfCare = Mapper.Map<EpisodeOfCare>(episodeOfCareDataIn);
            episodeOfCare.OrganizationId = userCookieData.GetActiveOrganizationData().Id;
            UserData userData = Mapper.Map<UserData>(userCookieData);

            return episodeOfCareDAL.InsertOrUpdate(episodeOfCare, userData);
        }

        private EpisodeOfCareFilter GetFilterData(EpisodeOfCareFilterDataIn dataIn, UserCookieData userCookieData)
        {
            EpisodeOfCareFilter result = Mapper.Map<EpisodeOfCareFilter>(dataIn);
            if (!string.IsNullOrEmpty(dataIn.IdentifierType) && !string.IsNullOrEmpty(dataIn.IdentifierValue))
            {
                result.FilterByIdentifier = true;
                result.PatientId = this.patientDAL.GetByIdentifier(new Identifier(dataIn.IdentifierType, dataIn.IdentifierValue)) != null ? this.patientDAL.GetByIdentifier(new Identifier(dataIn.IdentifierType, dataIn.IdentifierValue)).PatientId : 0;
            }

            result.OrganizationId = userCookieData.ActiveOrganization;
            return result;
        }
    }
}
