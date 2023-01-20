using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
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
    public class AdministrativeDataDAL : IAdministrativeDataDAL
    {
        private SReportsContext context;
        public AdministrativeDataDAL(SReportsContext context)
        {
            this.context = context;
        }

        public IEnumerable<AdministrativeData> GetAll()
        {
            return context.AdministrativeDatas;
        }

        public void InsertMany(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses)
        {
            DataTable administrativeDataTable = new DataTable();
            administrativeDataTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));

            for (int i = 0; i < thesauruses.Count; i++)
            {
                DataRow translationRow = administrativeDataTable.NewRow();
                translationRow["ThesaurusEntryId"] = bulkedThesauruses[i];

                administrativeDataTable.Rows.Add(translationRow);
            }

            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "AdministrativeDatas";
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");

            con.Open();
            objbulk.WriteToServer(administrativeDataTable);
            con.Close();
        }

        public void InsertManyVersions(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses)
        {
            DataTable versionTable = new DataTable();
            versionTable.Columns.Add(new DataColumn("Type", typeof(VersionType)));
            versionTable.Columns.Add(new DataColumn("CreatedOn", typeof(DateTime)));
            versionTable.Columns.Add(new DataColumn("State", typeof(ThesaurusState)));
            versionTable.Columns.Add(new DataColumn("AdministrativeDataId", typeof(int)));
            versionTable.Columns.Add(new DataColumn("UserId", typeof(int)));
            versionTable.Columns.Add(new DataColumn("OrganizationId", typeof(int)));

            int i = 0;
            foreach (var thesaurus in thesauruses)
            {
                if(thesaurus.AdministrativeData != null)
                {
                    foreach (var version in thesaurus.AdministrativeData.VersionHistory)
                    {
                        DataRow translationRow = versionTable.NewRow();
                        translationRow["Type"] = version.Type;
                        translationRow["CreatedOn"] = version.CreatedOn;
                        translationRow["State"] = version.State;
                        translationRow["AdministrativeDataId"] = bulkedThesauruses[i];
                        translationRow["UserId"] = version.UserId;
                        translationRow["OrganizationId"] = version.OrganizationId;

                        versionTable.Rows.Add(translationRow);
                    }
                }
                i++;
            }

            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;

            objbulk.DestinationTableName = "Versions";
            objbulk.ColumnMappings.Add("Type", "Type");
            objbulk.ColumnMappings.Add("CreatedOn", "CreatedOn");
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("AdministrativeDataId", "AdministrativeDataId");
            objbulk.ColumnMappings.Add("UserId", "UserId");
            objbulk.ColumnMappings.Add("OrganizationId", "OrganizationId");

            con.Open();
            objbulk.WriteToServer(versionTable);
            con.Close();
        }
    }
}
