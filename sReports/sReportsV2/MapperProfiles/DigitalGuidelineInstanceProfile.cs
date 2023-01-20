using AutoMapper;
using sReportsV2.Domain.Entities.DigitalGuidelineInstance;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class DigitalGuidelineInstanceProfile : Profile
    {
        public DigitalGuidelineInstanceProfile() 
        {
            CreateMap<GuidelineInstanceDataIn, GuidelineInstance>()
               .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(o => o.DigitalGuidelineId, opt => opt.MapFrom(src => src.DigitalGuidelineId))
               .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
               .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
               .ForMember(o => o.Title, opt => opt.MapFrom(src => src.Title))
               .ForMember(o => o.NodeValues, opt => opt.MapFrom(src => src.NodeValues))
               .ReverseMap();

            CreateMap<GuidelineInstance, GuidelineInstanceDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.DigitalGuidelineId, opt => opt.MapFrom(src => src.DigitalGuidelineId))
                .ForMember(o => o.EpisodeOfCareId, opt => opt.MapFrom(src => src.EpisodeOfCareId))
                .ForMember(o => o.Period, opt => opt.MapFrom(src => src.Period))
                .ForMember(o => o.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(o => o.NodeValues, opt => opt.MapFrom(src => src.NodeValues))
                .ReverseMap();
        }
    }
}