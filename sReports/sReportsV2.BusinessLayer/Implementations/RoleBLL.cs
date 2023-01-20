using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.RoleEntry;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class RoleBLL : IRoleBLL
    {
        private readonly IRoleDAL roleDAL;
        private readonly IPermissionRoleDAL permissionRoleDAL;
        private readonly IModuleDAL moduleDAL;

        public RoleBLL(IRoleDAL roleDAL, IModuleDAL moduleDAL, IPermissionRoleDAL permissionRoleDAL)
        {
            this.roleDAL = roleDAL;
            this.moduleDAL = moduleDAL;
            this.permissionRoleDAL = permissionRoleDAL;
        }

        public PaginationDataOut<RoleDataOut, DataIn> GetAll(DataIn dataIn)
        {
            Ensure.IsNotNull(dataIn, nameof(dataIn));

            RoleFilter filterData = Mapper.Map<RoleFilter>(dataIn);
            var result = new PaginationDataOut<RoleDataOut, DataIn>()
            { 
                Count = (int)roleDAL.GetAllFilteredCount(), 
                DataIn = dataIn, 
                Data = Mapper.Map<List<RoleDataOut>>(roleDAL.GetAll(filterData))
            };

            return result;
        }

        public List<RoleDataOut> GetAll()
        {
            return Mapper.Map<List<RoleDataOut>>(roleDAL.GetAll());
        }

        public List<ModuleDataOut> GetAllModules()
        {
            return Mapper.Map<List<ModuleDataOut>>(moduleDAL.GetAll());
        }

        public RoleDataOut GetById(int roleId)
        {
            return Mapper.Map<RoleDataOut>(roleDAL.GetById(roleId));
        }

        public CreateResponseResult InsertOrUpdate(RoleDataIn roleDataIn)
        {
            Role role = Mapper.Map<Role>(roleDataIn);
            Role roleDb = roleDAL.GetById(roleDataIn.Id) ?? role;
            if(roleDataIn.Id > 0)
            {
                roleDb.Copy(role);
                if (roleDb.PermissionsHaveChanged(role))
                {
                    permissionRoleDAL.DeletePermissionsForRole(roleDb.RoleId);
                    roleDb.Permissions = role.Permissions;
                }
            }

            roleDAL.InsertOrUpdate(roleDb);

            return new CreateResponseResult()
            {
                Id = roleDb.RoleId,
                RowVersion = roleDb.RowVersion
            };
        }
    }
}
