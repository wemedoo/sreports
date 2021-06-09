using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DAL.Sql.Interfaces;
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
    public class CodeableConceptService : ICodeableConceptService
    {
        public void InsertMany(List<ThesaurusEntry> thesauruses, Dictionary<string, int> bulkedThesauruses)
        {
            DataTable codesTable = new DataTable();
            codesTable.Columns.Add(new DataColumn("System", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Version", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Code", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Value", typeof(string)));
            codesTable.Columns.Add(new DataColumn("VersionPublishDate", typeof(DateTime)));
            codesTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));


            foreach (var thesaurus in thesauruses)
            {
                foreach (var code in thesaurus.Codes)
                {
                    DataRow codeRow = codesTable.NewRow();
                    codeRow["System"] = code.System;
                    codeRow["Version"] = code.Version;
                    codeRow["Code"] = code.Code;
                    codeRow["Value"] = code.Value;
                    codeRow["VersionPublishDate"] = code.VersionPublishDate;
                    //codeRow["ThesaurusEntryId"] = bulkedThesauruses[thesaurus.UmlsCode];

                    codesTable.Rows.Add(codeRow);
                }
            }

            string connection = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "O4CodeableConcept";
            objbulk.ColumnMappings.Add("System", "System");
            objbulk.ColumnMappings.Add("Version", "Version");
            objbulk.ColumnMappings.Add("Code", "Code");
            objbulk.ColumnMappings.Add("Value", "Value");
            objbulk.ColumnMappings.Add("VersionPublishDate", "VersionPublishDate");
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");

            con.Open();
            objbulk.WriteToServer(codesTable);
            con.Close();
        }
    }
}
