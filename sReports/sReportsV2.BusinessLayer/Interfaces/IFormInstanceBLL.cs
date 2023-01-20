using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataOut;
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
        string InsertOrUpdate(FormInstance form, FormInstanceStatus formInstanceStatus);
        FormInstance GetById(string id);
        List<FormInstance> GetByIds(List<string> ids);
        void Delete(string formInstanceId, DateTime lastUpdate);
        List<FormInstanceDataOut> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId);
        List<FormInstanceDataOut> SearchByTitle(int episodeOfCare, string title);
        void SignDocument(string formInstanceId, int signedById, FormState formInstanceState);
        IList<FormInstanceStatusDataOut> GetWorkflowHistory(List<FormInstanceStatus> formInstanceStatuses);
        Common.Helpers.DataCaptureChartUtility GetPlottableFieldsByThesaurusId(string formId, List<int> fieldThesaurusIds, List<FieldDataOut> fieldsDataOut, DateTime? DateTimeFrom, DateTime? DateTimeTo);
    }
}
