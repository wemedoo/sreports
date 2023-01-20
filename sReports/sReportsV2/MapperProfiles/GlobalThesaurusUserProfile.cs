using AutoMapper;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn;
using sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class GlobalThesaurusUserProfile : Profile
    {
        public GlobalThesaurusUserProfile() 
        {
            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataIn>()
                .ForMember(u => u.Roles , opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId))
                .ReverseMap();

            CreateMap<GlobalThesaurusUser, GlobalThesaurusUserDataOut>()
                .ForMember(d => d.Roles, opt => opt.MapFrom(src => src.Roles.Where(x => !x.IsDeleted).ToList()))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusUserId));

            CreateMap<GlobalThesaurusUserRole, RoleDataOut>()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.Role.GlobalThesaurusRoleId))
                .ForMember(u => u.Name, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(u => u.Description, opt => opt.MapFrom(src => src.Role.Description));

            CreateMap<GlobalThesaurusRole, RoleDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.GlobalThesaurusRoleId)); ;

        }
    }
}