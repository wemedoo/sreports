using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.EpisodeOfCare;
using System.Collections.Generic;

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


            CreateMap<EpisodeOfCareDataIn, EpisodeOfCareEntity>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.ListHistoryStatus, opt => opt.MapFrom(src => src.ListHistoryStatus))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<EpisodeOfCareEntity, EpisodeOfCareDataIn>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.ListHistoryStatus, opt => opt.MapFrom(src => src.ListHistoryStatus))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<FormEpisodeOfCare, EpisodeOfCareEntity>()
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ReverseMap();

            CreateMap<EpisodeOfCareDataOut, EpisodeOfCareEntity>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.ListHistoryStatus, opt => opt.MapFrom(src => src.ListHistoryStatus))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.OrganizationRef, opt => opt.MapFrom(src => src.OrganizationRef));

            CreateMap<EpisodeOfCareEntity, EpisodeOfCareDataOut>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
              .ForMember(o => o.DiagnosisCondition, opt => opt.MapFrom(src => src.DiagnosisCondition))
              .ForMember(o => o.DiagnosisRole, opt => opt.MapFrom(src => src.DiagnosisRole))
              .ForMember(o => o.DiagnosisRank, opt => opt.MapFrom(src => src.DiagnosisRank))
              .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
              .ForMember(o => o.ListHistoryStatus, opt => opt.MapFrom(src => src.ListHistoryStatus))
              .ForMember(o => o.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.OrganizationRef, opt => opt.MapFrom(src => src.OrganizationRef))
              .ForMember(o => o.DiagnosticReports, opt => opt.Ignore())
              .ForMember(o => o.Patient, opt => opt.Ignore());

            CreateMap<EpisodeOfCareFilterDataIn, EpisodeOfCareFilter>();
        }

    }
}