using AutoMapper;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {

            CreateMap<TelecomDTO, Telecom>()
                .ForMember(d => d.TelecomId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<Address, AddressDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.AddressId))
                .AfterMap((entity, dto) => {
                    dto.Country = SingletonDataContainer.Instance.GetCustomEnumPreferredTerm(entity.CountryId.GetValueOrDefault());
                })
                .ReverseMap();

            CreateMap<AddressDataIn, Address>()
                .ForMember(d => d.AddressId, opt => opt.MapFrom(src => src.Id));

            //CreateMap<AddressDTO, Address>()
            //    .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Identifier, IdentifierDataIn>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.IdentifierId))
                .ReverseMap();

            CreateMap<Identifier, IdentifierDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.IdentifierId))
                .ForMember(o => o.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(o => o.Use, opt => opt.MapFrom(src => src.Use))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.System, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetEnums().FirstOrDefault(x => x.Thesaurus.Id.ToString() == src.System)))
                .ReverseMap();

            CreateMap<Name, NameDTO>()
                .ReverseMap();
        }
    }
}