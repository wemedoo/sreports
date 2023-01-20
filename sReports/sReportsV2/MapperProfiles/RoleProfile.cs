using AutoMapper;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleDataIn, Role>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PermissionRoleDataIn, PermissionRole>()
                .ForMember(d => d.PermissionRoleId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Role, RoleDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.RoleId));

            CreateMap<PermissionRole, PermissionRoleDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PermissionRoleId))
                .ForMember(d => d.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
                .ForMember(d => d.ModuleName, opt => opt.MapFrom(src => src.Module.Name))
                .ForMember(d => d.PermissionId, opt => opt.MapFrom(src => src.PermissionId))
                .ForMember(d => d.Permission, opt => opt.MapFrom(src => src.Permission))
                .ForAllOtherMembers(x => x.Ignore())
                ;

            CreateMap<Module, ModuleDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.ModuleId));

            CreateMap<Permission, PermissionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PermissionId)); ;

            CreateMap<PermissionModule, PermissionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.PermissionId))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Permission.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Permission.Description))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<DataIn, RoleFilter>();

        }
    }
}