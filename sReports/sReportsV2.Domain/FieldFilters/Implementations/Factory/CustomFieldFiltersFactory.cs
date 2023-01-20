using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using System;

namespace sReportsV2.Domain.FieldFilters.Implementations
{
    public static class CustomFieldFiltersFactory
    {
        public static CustomFieldFilter Create(CustomFieldFilterData fieldFilter)
        {
            switch (fieldFilter.FieldType)
            {
                case FieldTypes.Text:
                case FieldTypes.LongText:
                case FieldTypes.Radio:
                case FieldTypes.URL:
                case FieldTypes.Email:
                case FieldTypes.Checkbox:
                case FieldTypes.Select:
                case FieldTypes.File:
                case FieldTypes.Regex:
                case FieldTypes.CustomButton:
                    {
                        return new TextualCustomFieldFilter(fieldFilter.FieldThesaurusId, fieldFilter.Value, fieldFilter.FilterOperator);
                    }
                case FieldTypes.Number:
                case FieldTypes.Digits:
                case FieldTypes.Calculative:
                    {
                        return new NumericCustomFieldFilter(fieldFilter.FieldThesaurusId, fieldFilter.Value, fieldFilter.FilterOperator);
                    }
                case FieldTypes.Date:
                case FieldTypes.Datetime:
                    {
                        return new DateCustomFieldFilter(fieldFilter.FieldThesaurusId, fieldFilter.Value, fieldFilter.FilterOperator);
                    }
                default:
                    throw new ArgumentException($"{fieldFilter.FieldType} Field Filter Type not supported");

            }
        }
    }
}
