using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.FieldFilters.Enum
{
    public static class FieldOperators
    {
        // Equality Operators
        public const string Equal = "equal";
        public const string NotEqual = "not equal";
        // Comparison Operators
        public const string GreaterThanOrEqual = "greater than or equal";
        public const string LessThanOrEqual = "less than or equal";
        public const string GreaterThan = "greater than";
        public const string LessThan = "less than";
        // Text-Only Operators
        public const string Contains = "contains";
    }
}
