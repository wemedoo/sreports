using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFormInstanceBLL
    {
        PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn> ReloadData(FormInstanceFilterDataIn dataIn);
        string InsertOrUpdate(FormInstance form);
        FormInstance GetById(string id);
        List<FormInstance> GetByIds(List<string> ids);
        void Delete(string formInstanceId, DateTime lastUpdate);
    }
}
