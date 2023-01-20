using MongoDB.Driver;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FieldFilters.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.FieldFilters.Implementations
{
    public class TextualCustomFieldFilter : CustomFieldFilter
    {
        public TextualCustomFieldFilter(int fieldThesaurusId, string value, string filterOperator)
        {
            Filter = AssembleFilter(fieldThesaurusId, value, filterOperator);
        }

        protected override FilterDefinition<FormInstance> AssembleFilter(int fieldThesaurusId, string valueToFilter, string filterOperator)
        {
            switch (filterOperator)
            {
                // Equality Operators
                case FieldOperators.Equal: return EqualityFiltersBuilder.Equal(valueToFilter, fieldThesaurusId);
                case FieldOperators.NotEqual: return EqualityFiltersBuilder.NotEqual(valueToFilter, fieldThesaurusId);
                // Text-Only Operators
                case FieldOperators.Contains: return TextualFiltersBuilder.Contains(valueToFilter, fieldThesaurusId);

                default: throw new ArgumentException($"{filterOperator} is not a supported Field Filter Operator for {this.GetType().Name} class");
            }
        }
    }
}
