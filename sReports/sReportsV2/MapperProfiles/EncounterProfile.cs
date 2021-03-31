using AutoMapper;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Common;
using sReportsV2.Domain.Entities.Common;
using System.Collections.Generic;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.Models.ThesaurusEntry;
using System.Globalization;

namespace sReportsV2.MapperProfiles
{
    public class EncounterProfile : Profile
    {
        public EncounterProfile()
        {
            CreateMap<EnumEntry, EnumDataIn>()
                .ForMember(o => o.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ReverseMap()
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EnumEntry, EnumDataOut>()
                .ReverseMap()
                .ForAllOtherMembers(opts => opts.Ignore());


            CreateMap<EncounterEntity, EncounterDataIn>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterDataIn, EncounterEntity>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterEntity, EncounterDataOut>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => new ServiceType { Display = src.ServiceType }))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(o => o.FormInstances, opt => opt.MapFrom(src => src.FormInstances))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, EnumDataOut>()
                .ForMember(o => o.Thesaurus, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());
            CreateMap<string, ThesaurusEntryViewModel>()
                .ForMember(o => o.O40MTId, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterDataOut, EncounterEntity>()
                .ForMember(o => o.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type.Thesaurus.O40MTId))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType.Display))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterFilterDataIn, EncounterFilter>()
                 .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                 .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                 .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                 .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterFilter, EncounterFilterDataIn>()
                 .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                 .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                 .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                 .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                 .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<ServiceType, ServiceTypeDTO>()
                .ForMember(o => o.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(o => o.Display, opt => opt.MapFrom(src => src.Display))
                .ForMember(o => o.Definition, opt => opt.MapFrom(src => src.Definition))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<ServiceTypeDTO, ServiceType>()
                .ForMember(o => o.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(o => o.Display, opt => opt.MapFrom(src => src.Display))
                .ForMember(o => o.Definition, opt => opt.MapFrom(src => src.Definition))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}