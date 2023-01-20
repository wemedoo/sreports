using MongoDB.Driver;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.FieldFilters
{
    public abstract class CustomFieldFilter
    {
        public FilterDefinition<FormInstance> Filter { get; set; }

        protected abstract FilterDefinition<FormInstance> AssembleFilter(int fieldThesaurusId, string valueToFilter, string filterOperator);

    }
}
