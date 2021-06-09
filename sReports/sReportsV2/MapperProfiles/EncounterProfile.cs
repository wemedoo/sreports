using AutoMapper;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.DTOs.Encounter;
using sReportsV2.DTOs.Common;
using sReportsV2.Domain.Entities.Common;
using System.Collections.Generic;
using sReportsV2.DTOs.Encounter.DataOut;
using System.Globalization;
using sReportsV2.DTOs.Patient;
using sReportsV2.Common.Singleton;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Encounter;

namespace sReportsV2.MapperProfiles
{
    public class EncounterProfile : Profile
    {
        public EncounterProfile()
        {
            CreateMap<EncounterEntity, EncounterDataIn>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterDataIn, EncounterEntity>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterEntity, EncounterDataOut>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.ServiceType).FirstOrDefault(x => x.Thesaurus.Id.Equals(src.ServiceType))))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(o => o.FormInstances, opt => opt.MapFrom(src => src.FormInstances))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<int, ThesaurusEntryDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());
            
            CreateMap<EncounterDataOut, EncounterEntity>()
                .ForMember(o => o.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type.Thesaurus.Id))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
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

            CreateMap<Encounter, EncounterDataIn>()
               .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
               .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
               .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
               .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
               .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
               .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterDataIn, Encounter>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Encounter, EncounterDataOut>()
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetEnums().Where(x => x.Type == CustomEnumType.ServiceType).FirstOrDefault(x => x.Thesaurus.Id.Equals(src.ServiceType))))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EncounterDataOut, Encounter>()
                .ForMember(o => o.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(o => o.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type.Thesaurus.Id))
                .ForMember(o => o.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(o => o.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}