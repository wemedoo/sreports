using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class SimilarTermService : ISimilarTermService
    {
        public List<SimilarTermSearch> GetTermFiltered(List<string> terms)
        {
            List<SimilarTermSearch> result = new List<SimilarTermSearch>();
           string sql = $"SELECT [Name],[ThesaurusEntryTranslationId] FROM [SimilarTerms] where contains([Name], '{MakeSearchString(terms)}')";
            using (var context = new SReportsContext()) 
            {
                context.Database.CommandTimeout = 0;
                result = context.Database.SqlQuery<SimilarTermSearch>(sql).ToList();
            }

            return result;
        }
        private string MakeSearchString(List<string> terms)
        {
            string result = $"\"{terms[0]}\"";
            for (int i = 1; i < terms.Count(); i++)
            {
                result += $"or \"{terms[i]}\"";
            }
            return result;
        }
        public void InsertMany(List<ThesaurusEntry> thesauruses, List<TranslationBulkInfo> bulkedTranslations, Dictionary<string, int> bulkedThesauruses)
        {
            
        }
    }
}
