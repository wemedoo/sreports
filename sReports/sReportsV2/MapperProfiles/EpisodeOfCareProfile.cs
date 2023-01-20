using AutoMapper;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;

namespace sReportsV2.MapperProfiles
{
    public class EpisodeOfCareProfile : Profile
    {
        public EpisodeOfCareProfile()
        {
            CreateMap<Period, PeriodDTO>()
              .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.Start))
              .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<PeriodDTO, Period>()
              .ForMember(o => o.Start, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(o => o.End, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<PeriodDatetime, PeriodDTO>()
              .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.Start))
              .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<PeriodDTO, PeriodDatetime>()
              .ForMember(o => o.Start, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(o => o.End, opt => opt.MapFrom(src => src.EndDate));

            
            CreateMap<EpisodeOfCareFilterDataIn, EpisodeOfCareFilter>();

            CreateMap<Domain.Sql.Entities.Common.PeriodDatetime, PeriodDTO>()
              .ForMember(o => o.StartDate, opt => opt.MapFrom(src => src.Start))
              .ForMember(o => o.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<PeriodDTO, Domain.Sql.Entities.Common.PeriodDatetime>()
              .ForMember(o => o.Start, opt => opt.MapFrom(src => src.StartDate))
              .ForMember(o => o.End, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<EpisodeOfCareDataIn,EpisodeOfCare>()
              .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<EpisodeOfCare, EpisodeOfCareDataIn>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.EpisodeOfCareId))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<EpisodeOfCareDataOut, EpisodeOfCare>()
              .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<EpisodeOfCare, EpisodeOfCareDataOut>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.EpisodeOfCareId))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));
        }

    }
}