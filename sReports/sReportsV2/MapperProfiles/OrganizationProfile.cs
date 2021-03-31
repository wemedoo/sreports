using AutoMapper;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using System.Collections.Generic;

namespace sReportsV2.MapperProfiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationDataOut, Domain.Entities.OrganizationEntities.Organization>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src =>src.Id))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(o => o.Telecom, opt => opt.MapFrom(src => src.Telecom))
                .ForMember(o => o.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(o => o.Activity, opt => opt.MapFrom(src => src.Activity))
                .ForMember(o => o.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(o => o.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(o => o.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PartOf, opt => opt.Ignore())
                .ForMember(o => o.Ancestors, opt => opt.Ignore());

            CreateMap<Domain.Entities.OrganizationEntities.Organization, OrganizationDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(o => o.Telecom, opt => opt.MapFrom(src => src.Telecom))
                .ForMember(o => o.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(o => o.Activity, opt => opt.MapFrom(src => src.Activity))
                .ForMember(o => o.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(o => o.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(o => o.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.PartOf, opt => opt.Ignore())
                .ForMember(o => o.Ancestors, opt => opt.Ignore());

            CreateMap<OrganizationDataIn, Domain.Entities.OrganizationEntities.Organization>()
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Domain.Entities.OrganizationEntities.Organization, OrganizationDataIn>()
                .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<IdentifierEntity, IdentifierDataIn>().ReverseMap();

            CreateMap<IdentifierEntity, IdentifierDataOut>()
                .ForMember(o => o.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(o => o.Use, opt => opt.MapFrom(src => src.Use))
                .ForMember(o => o.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(o => o.System, opt => opt.MapFrom(src => src.System))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<OrganizationFilterDataIn, OrganizationFilter>();

            CreateMap<OrganizationUsersCount, OrganizationUsersCountDataOut>()
                .ForMember(o => o.Children, opt => opt.MapFrom(src => src.Children))
                .ForMember(o => o.OrganizationName, opt => opt.MapFrom(src => src.OrganizationName))
                .ForMember(o => o.UsersCount, opt => opt.MapFrom(src => src.UsersCount))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}