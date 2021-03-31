using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.Models.Common;
using sReportsV2.Models.User;

namespace sReportsV2.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserCookieData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.Organizations, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Organization, OrganizationDataOut>().ReverseMap();

            CreateMap<OrganizationDataOut, Organization>().ReverseMap();

            CreateMap<UserCookieData, UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.OrganizationRefs, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserCookieData, UserDataViewModel>().ReverseMap();

            CreateMap<UserCookieData, UserData>().ReverseMap();

            CreateMap<UserDataViewModel, UserData>().ReverseMap();

            CreateMap<UserData, UserDataDataOut>().ReverseMap();
            
            CreateMap<UserCookieData, UserDataDataOut>().ReverseMap();

            CreateMap<User, UserDataDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
               .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
               .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(d => d.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
               .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles))
               .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.OrganizationRefs))
               .ForAllOtherMembers(opts => opts.Ignore());
            
            CreateMap<string, UserDataDataOut>()
               .ForMember(o => o.Organizations, opt => opt.MapFrom(src => src))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, OrganizationDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<User, UserDataDataIn>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.OrganizationRefs))
                .ReverseMap();

        }
    }
}