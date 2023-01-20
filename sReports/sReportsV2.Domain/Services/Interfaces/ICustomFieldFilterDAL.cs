using sReportsV2.Domain.Entities.CustomFieldFilters;
using System.Collections.Generic;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface ICustomFieldFilterDAL
    {
        string InsertOrUpdateCustomFieldFilter(CustomFieldFilterGroup dataToSave);
        List<CustomFieldFilterGroup> GetCustomFieldFiltersByFormId(string formDefinitionId);
    }
}
