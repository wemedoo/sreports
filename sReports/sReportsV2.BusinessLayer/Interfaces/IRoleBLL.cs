using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.AccessManagment.DataIn;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IRoleBLL
    {
        PaginationDataOut<RoleDataOut, DataIn> GetAll(DataIn dataIn);
        List<RoleDataOut> GetAll();

        RoleDataOut GetById(int roleId);
        CreateResponseResult InsertOrUpdate(RoleDataIn roleDataIn);
        List<ModuleDataOut> GetAllModules();
    }
}
