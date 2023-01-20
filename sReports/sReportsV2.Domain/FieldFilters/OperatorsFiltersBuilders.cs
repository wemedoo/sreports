using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FormValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.FieldFilters
{
    internal static class CommonFilters
    {
        internal static FilterDefinition<FieldValue> FieldThesaurusIdFilter(int fieldThesaurusId)
        {
            return Builders<FieldValue>.Filter.Eq(x => x.ThesaurusId, fieldThesaurusId);
        }
    }

    public static class EqualityFiltersBuilder 
    {
        public static FilterDefinition<FormInstance> Equal(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyEq(x => x.ValueLabel, value)
                );
        }

        public static FilterDefinition<FormInstance> NotEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyNe(x => x.ValueLabel ,value)
                );
        }
    }

    public static class TextualFiltersBuilder 
    {
        public static FilterDefinition<FormInstance> Contains(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId) 
                & Builders<FieldValue>.Filter.Regex(x => x.ValueLabel, new BsonRegularExpression(value, "i"))  // i: case insensitive option 
                );
        }
    }

    public static class ComparisonFiltersBuilder
    {
        public static FilterDefinition<FormInstance> LessThan(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyLt(x => x.ValueLabel, value)
                );
        }

        public static FilterDefinition<FormInstance> LessThanOrEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyLt(x => x.ValueLabel, value)
                );
        }

        public static FilterDefinition<FormInstance> GreaterThanOrEqual(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyGt(x => x.ValueLabel, value)
                );
        }

        public static FilterDefinition<FormInstance> GreaterThan(string value, int fieldThesaurusId)
        {
            return Builders<FormInstance>.Filter.ElemMatch(
                x => x.Fields,
                CommonFilters.FieldThesaurusIdFilter(fieldThesaurusId)
                & Builders<FieldValue>.Filter.AnyGte(x => x.ValueLabel, value)
                );
        }
    }
}
