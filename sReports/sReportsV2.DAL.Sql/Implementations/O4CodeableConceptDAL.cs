using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class O4CodeableConceptDAL : IO4CodeableConceptDAL
    {
        public void InsertMany(List<ThesaurusEntry> thesauruses,List<int> bulkedThesauruses)
        {
            DataTable codesTable = new DataTable();
            codesTable.Columns.Add(new DataColumn("Version", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Code", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Value", typeof(string)));
            codesTable.Columns.Add(new DataColumn("Link", typeof(string)));
            codesTable.Columns.Add(new DataColumn("VersionPublishDate", typeof(DateTime)));
            codesTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));
            codesTable.Columns.Add(new DataColumn("CodeSystemId", typeof(int)));
            codesTable.Columns.Add(new DataColumn("IsDeleted", typeof(bool)));
            codesTable.Columns.Add(new DataColumn("EntryDateTime", typeof(DateTime)));

            int i = 0;
            foreach (var thesaurus in thesauruses)
            {
                foreach (var code in thesaurus.Codes)
                {
                    DataRow codeRow = codesTable.NewRow();
                    codeRow["Version"] = code.Version;
                    codeRow["Code"] = code.Code;
                    codeRow["Value"] = code.Value;
                    codeRow["Link"] = code.Link;
                    codeRow["VersionPublishDate"] = code.VersionPublishDate;
                    codeRow["ThesaurusEntryId"] = bulkedThesauruses[i];
                    codeRow["CodeSystemId"] = code.CodeSystemId;
                    codeRow["EntryDateTime"] = code.EntryDateTime;
                    codeRow["IsDeleted"] = false;
                    codesTable.Rows.Add(codeRow);
                }

                i++;
            }

            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "O4CodeableConcept";
            objbulk.ColumnMappings.Add("Version", "Version");
            objbulk.ColumnMappings.Add("Code", "Code");
            objbulk.ColumnMappings.Add("Value", "Value");
            objbulk.ColumnMappings.Add("VersionPublishDate", "VersionPublishDate");
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");
            objbulk.ColumnMappings.Add("CodeSystemId", "CodeSystemId");
            objbulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
            objbulk.ColumnMappings.Add("EntryDateTime", "EntryDateTime");
            objbulk.ColumnMappings.Add("Link", "Link");

            con.Open();
            objbulk.WriteToServer(codesTable);
            con.Close();
        }
    }
}
