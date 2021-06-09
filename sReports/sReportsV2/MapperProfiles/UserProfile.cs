using AutoMapper;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<GlobalThesaurusUser, UserCookieData>()
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<User, UserCookieData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.ActiveLanguage, opt => opt.MapFrom(src => src.UserConfig.ActiveLanguage))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.UserConfig.ActiveOrganizationId))
                .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.UserConfig.PageSize))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(o => o.Organizations.Select(x => x.Organization)))
                .ForMember(d => d.TimeZoneOffset, opt => opt.MapFrom(o => o.UserConfig.TimeZoneOffset))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<UserOrganization, UserOrganizationDataOut>()
             .ForMember(d => d.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
             .ForMember(d => d.IsPracticioner, opt => opt.MapFrom(src => src.IsPracticioner))
             .ForMember(d => d.LegalForm, opt => opt.MapFrom(src => src.LegalForm))
             .ForMember(d => d.Organization, opt => opt.MapFrom(src => src.Organization))
             .ForMember(d => d.OrganizationalForm, opt => opt.MapFrom(src => src.OrganizationalForm))
             .ForMember(d => d.Qualification, opt => opt.MapFrom(src => src.Qualification))
             .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles))
             .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(src => src.SeniorityLevel))
             .ForMember(d => d.Speciality, opt => opt.MapFrom(src => src.Speciality))
             .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
             .ForMember(d => d.SubSpeciality, opt => opt.MapFrom(src => src.SubSpeciality))
             .ForMember(d => d.Team, opt => opt.MapFrom(src => src.Team))
             .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UserOrganization, UserOrganizationDataIn>().ReverseMap();


            CreateMap<UserCookieData, UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.Organizations, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserCookieData, UserShortInfoDataOut>().ReverseMap();

            CreateMap<UserCookieData, UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.ActiveOrganization))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.Organizations.Select(x => x.Id).ToList()))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UserCookieData, Common.Entities.User.UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.ActiveOrganization))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.Organizations.Select(x => x.Id).ToList()))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UserShortInfoDataOut, UserData>().ReverseMap();
            CreateMap<UserShortInfoDataOut, User>().ReverseMap();

            CreateMap<UserData, UserDataOut>().ReverseMap();
            CreateMap<UserDataIn, User>()
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.UserOrganizations))
                .ReverseMap();

            CreateMap<UserCookieData, UserDataOut>()
                 .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                 .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                 .ForAllOtherMembers(x => x.Ignore());

            CreateMap<User, UserDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
               .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
               .ForMember(d => d.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
               .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles))
               .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.GetOrganizationRefs()))
               .ForMember(d => d.Prefix, opt => opt.MapFrom(src => src.Prefix))
               .ForMember(d => d.AcademicPositions, opt => opt.MapFrom(src => src.AcademicPositions))
               .ForMember(d => d.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(d => d.DayOfBirth, opt => opt.MapFrom(src => src.DayOfBirth))
               .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.Organizations))
               .ForMember(o => o.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(d => d.ClinicalTrials, opt => opt.MapFrom(src => src.ClinicalTrials))
               .ForAllOtherMembers(opts => opts.Ignore());
            
            CreateMap<string, UserDataOut>()
               .ForMember(o => o.Organizations, opt => opt.MapFrom(src => src))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, OrganizationDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserClinicalTrial, ClinicalTrialDTO>().ReverseMap();
        }
    }
}