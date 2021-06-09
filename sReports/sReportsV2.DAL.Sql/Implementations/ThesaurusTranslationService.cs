using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
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
    public class ThesaurusTranslationService : IThesaurusTranslationService
    {
        public List<TranslationBulkInfo> GetLastBulkInserted(int size)
        {
            List<TranslationBulkInfo> result = new List<TranslationBulkInfo>();
            using (var context = new SReportsContext())
            {
                result = context.Thesauruses
                     .OrderByDescending(x => x.EntryDatetime)
                     .Take(size)
                     .SelectMany(x => x.Translations.Select(z => new TranslationBulkInfo() 
                     {
                        Id = z.Id,
                        ThesaurusId = x.Id
                     })).ToList();
            }

            return result;
        }

        public void InsertMany(List<ThesaurusEntry> thesauruses, Dictionary<string, int> bulkedThesauruses)
        {
            DataTable translationTable = new DataTable();
            translationTable.Columns.Add(new DataColumn("Language", typeof(string)));
            translationTable.Columns.Add(new DataColumn("Definition", typeof(string)));
            translationTable.Columns.Add(new DataColumn("PreferredTerm", typeof(string)));
            translationTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));

            foreach (var thesaurus in thesauruses) 
            {
                foreach (var translation in thesaurus.Translations) 
                {
                    DataRow translationRow = translationTable.NewRow();
                    translationRow["Language"] = translation.Language;
                    translationRow["Definition"] = translation.Definition;
                    translationRow["PreferredTerm"] = translation.PreferredTerm;
                    //translationRow["ThesaurusEntryId"] = bulkedThesauruses[thesaurus.UmlsCode];

                    translationTable.Rows.Add(translationRow);
                }
            }

            string connection = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "ThesaurusEntryTranslations";
            objbulk.ColumnMappings.Add("Language", "Language");
            objbulk.ColumnMappings.Add("Definition", "Definition");
            objbulk.ColumnMappings.Add("PreferredTerm", "PreferredTerm");
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");

            con.Open();
            objbulk.WriteToServer(translationTable);
            con.Close();
        }
    }
}
