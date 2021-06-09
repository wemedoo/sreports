using AutoMapper;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {

            CreateMap<TelecomDTO, Telecom>()
                .ReverseMap();

            CreateMap<Address, AddressDTO>()
                .ReverseMap();

            //CreateMap<AddressDTO, Address>()
            //    .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Identifier, IdentifierDataIn>().ReverseMap();

            CreateMap<Identifier, IdentifierDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(o => o.Use, opt => opt.MapFrom(src => src.Use))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.System, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetEnums().FirstOrDefault(x => x.Thesaurus.Id.ToString() == src.System)))
                .ReverseMap();
        }
    }
}