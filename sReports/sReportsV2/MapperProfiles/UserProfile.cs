using AutoMapper;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<GlobalThesaurusUser, UserCookieData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles.Where(x => !x.IsDeleted).ToList()))
                .ReverseMap();

            CreateMap<User, UserCookieData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.ActiveLanguage, opt => opt.MapFrom(src => src.UserConfig.ActiveLanguage))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.UserConfig.ActiveOrganizationId))
                .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.UserConfig.PageSize))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles.Where(x => !x.IsDeleted).ToList()))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(o => o.GetActiveOrganizations().Select(x => x.Organization)))
                .ForMember(d => d.TimeZoneOffset, opt => opt.MapFrom(o => o.UserConfig.TimeZoneOffset))
                .ForMember(d => d.SuggestedForms, opt => opt.MapFrom(o => o.UserConfig.GetForms()))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<UserOrganization, UserOrganizationDataOut>()
                .ForMember(d => d.IsPracticioner, opt => opt.MapFrom(src => src.IsPracticioner))
                .ForMember(d => d.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(d => d.Qualification, opt => opt.MapFrom(src => src.Qualification))
                .ForMember(d => d.SeniorityLevel, opt => opt.MapFrom(src => src.SeniorityLevel))
                .ForMember(d => d.Speciality, opt => opt.MapFrom(src => src.Speciality))
                .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
                .ForMember(d => d.SubSpeciality, opt => opt.MapFrom(src => src.SubSpeciality))
                .ForAllOtherMembers(x => x.Ignore());


            CreateMap<UserOrganization, UserOrganizationDataIn>()
                .ReverseMap();

            CreateMap<UserCookieData, UserShortInfoDataOut>()
                .ReverseMap();

            CreateMap<UserCookieData, UserData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.ActiveOrganization, opt => opt.MapFrom(src => src.ActiveOrganization))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.Organizations.Select(x => x.Id).ToList()))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<UserShortInfoDataOut, UserData>()
                .ReverseMap();

            CreateMap<UserShortInfoDataOut, User>()
                .ReverseMap();

            CreateMap<UserData, UserDataOut>()
                .ReverseMap();

            CreateMap<UserDataIn, User>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Roles, opt => opt.Ignore())
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.UserOrganizations))
                .ForMember(d => d.AcademicPositions, opt => opt.Ignore())
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username.TrimInput()))
                .ReverseMap();
          
            CreateMap<UserCookieData, UserDataOut>()
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<User, UserDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(d => d.ContactPhone, opt => opt.MapFrom(src => src.ContactPhone))
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles.Where(x => !x.IsDeleted).ToList()))
                .ForMember(d => d.Prefix, opt => opt.MapFrom(src => src.Prefix))
                .ForMember(d => d.AcademicPositions, opt => opt.MapFrom(src => src.AcademicPositions.Where(x => !x.IsDeleted).Select(x => (AcademicPosition)x.AcademicPositionTypeId).ToList()))
                .ForMember(d => d.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(d => d.DayOfBirth, opt => opt.MapFrom(src => src.DayOfBirth))
                .ForMember(d => d.Organizations, opt => opt.MapFrom(src => src.GetNonArchivedOrganizations()))
                .ForMember(o => o.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.PersonalEmail, opt => opt.MapFrom(src => src.PersonalEmail))
                .ForMember(d => d.ClinicalTrials, opt => opt.MapFrom(src => src.ClinicalTrials))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserRole, RoleDataOut>()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.Role.RoleId))
                .ForMember(u => u.Name, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(u => u.Description, opt => opt.MapFrom(src => src.Role.Description))
                .ForMember(u => u.Permissions, opt => opt.MapFrom(src => src.Role.Permissions));

            CreateMap<string, UserDataOut>()
                .ForMember(o => o.Organizations, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, OrganizationDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserClinicalTrial, ClinicalTrialDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserClinicalTrialId))
                .ReverseMap();

            CreateMap<User, UserData>()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(u => u.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(u => u.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(u => u.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<UserFilterDataIn, UserFilter>();

            CreateMap<UserView, UserViewDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();
        }
    }
}