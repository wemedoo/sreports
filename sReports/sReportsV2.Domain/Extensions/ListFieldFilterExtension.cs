using MongoDB.Driver;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FieldFilters;
using System.Collections.Generic;

namespace sReportsV2.Domain.Extensions
{
    public static class ListFieldFilterExtension
    {
        public static FilterDefinition<FormInstance> AllAnd(this List<CustomFieldFilter> filters)
        {
            FilterDefinition<FormInstance> combinedFilters = FilterDefinition<FormInstance>.Empty;
            for (int i = 0; i < filters.Count; i++)
            {
                if (i == 0)
                    combinedFilters = filters[i].Filter;
                else
                    combinedFilters &= filters[i].Filter;
            }

            return combinedFilters;
        }

        public static FilterDefinition<FormInstance> AllOr(this List<CustomFieldFilter> filters)
        {
            FilterDefinition<FormInstance> combinedFilters = FilterDefinition<FormInstance>.Empty;
            for (int i = 0; i < filters.Count; i++)
            {
                if (i == 0)
                    combinedFilters = filters[i].Filter;
                else
                    combinedFilters |= filters[i].Filter;
            }

            return combinedFilters;
        }
    }
}
