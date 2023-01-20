using AutoMapper;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.CustomEnum;
using sReportsV2.DTOs.CustomEnum.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class CustomEnumProfile : Profile
    {
        public CustomEnumProfile()
        {
            CreateMap<CustomEnum, CustomEnumDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.CustomEnumId))
                .ForMember(o => o.Thesaurus, opt => opt.MapFrom(src => src.ThesaurusEntry))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<CustomEnumDataOut, CustomEnum>()
                .ForMember(o => o.ThesaurusEntryId, opt => opt.MapFrom(src => src.Thesaurus.Id))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(d => d.CustomEnumId, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<int, CustomEnumDataOut>()
                .ForMember(o => o.Thesaurus, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetEnums().FirstOrDefault(x => x.Thesaurus.Id == src).Thesaurus))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<CustomEnumDataIn, CustomEnum>()
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.CustomEnumId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(o => o.ThesaurusEntryId, opt => opt.MapFrom(src => src.ThesaurusEntryId))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}