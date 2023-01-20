using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.Helpers
{
    public static class SortTableHelper
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string sortField, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            MemberExpression prop = GetMemberExpression(parameter, sortField);
            var expression = Expression.Lambda(prop, parameter);
            string method = ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, expression.Body.Type };
            var methodCallExpression = Expression.Call(typeof(Queryable), method, types, q.Expression, expression);
            return q.Provider.CreateQuery<T>(methodCallExpression);
        }

        private static MemberExpression GetMemberExpression(ParameterExpression param, string sortField) 
        {
            string[] property = sortField.Split('.');
            MemberExpression prop = Expression.Property(param, property[0]);

            if (property.Count() > 1)
                prop = Expression.Property(prop, property[1]);

            return prop;
        }
    }
}
