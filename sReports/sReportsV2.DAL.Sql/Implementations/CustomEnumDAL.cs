using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class CustomEnumDAL : ICustomEnumDAL
    {
        private readonly SReportsContext context;
        public CustomEnumDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool CustomEnumExist(CustomEnumType customEnumType)
        {
            return context.CustomEnums.Any(c => !c.IsDeleted && c.Type == customEnumType);
        }

        public void Delete(CustomEnum customEnum)
        {
            CustomEnum fromDb = context.CustomEnums.FirstOrDefault(x => x.CustomEnumId == customEnum.CustomEnumId);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public IQueryable<CustomEnum> GetAll()
        {
            return context.CustomEnums.Where(x => !x.IsDeleted)
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations)
                .Include(x => x.Organization);
        }

        public int? GetIdByTypeAndPreferredTerm(string preferredTerm, CustomEnumType customEnumType)
        {
            return context.CustomEnums.Where(e => !e.IsDeleted && customEnumType == e.Type && e.ThesaurusEntry.Translations.Any(t => t.PreferredTerm == preferredTerm)).Select(x => x.CustomEnumId).FirstOrDefault();
        }

        public void Insert(CustomEnum customEnum)
        {
            if (customEnum.CustomEnumId == 0)
            {
                context.CustomEnums.Add(customEnum);
            }
            else
            {
                customEnum.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public void InsertMany(List<int> bulkedThesauruses, int organizationId, CustomEnumType customEnumType)
        {
            DataTable customEnumDataTable = new DataTable();
            customEnumDataTable.Columns.Add(new DataColumn("ThesaurusEntryId", typeof(int)));
            customEnumDataTable.Columns.Add(new DataColumn("OrganizationId", typeof(int)));
            customEnumDataTable.Columns.Add(new DataColumn("Type", typeof(CustomEnumType)));
            customEnumDataTable.Columns.Add(new DataColumn("IsDeleted", typeof(bool)));
            customEnumDataTable.Columns.Add(new DataColumn("Active", typeof(bool)));
            customEnumDataTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTime)));
            customEnumDataTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTime)));

            for (int i = 0; i < bulkedThesauruses.Count; i++)
            {
                DataRow translationRow = customEnumDataTable.NewRow();
                translationRow["ThesaurusEntryId"] = bulkedThesauruses[i];
                translationRow["OrganizationId"] = organizationId;
                translationRow["Type"] = customEnumType;
                translationRow["IsDeleted"] = false;
                translationRow["Active"] = true;
                translationRow["EntryDatetime"] = DateTime.Now;
                translationRow["LastUpdate"] = DBNull.Value;

                customEnumDataTable.Rows.Add(translationRow);
            }

            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con)
            {
                BulkCopyTimeout = 0,
                DestinationTableName = "CustomEnums"
            };
            objbulk.ColumnMappings.Add("ThesaurusEntryId", "ThesaurusEntryId");
            objbulk.ColumnMappings.Add("Type", "Type");
            objbulk.ColumnMappings.Add("OrganizationId", "OrganizationId");
            objbulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
            objbulk.ColumnMappings.Add("Active", "Active");
            objbulk.ColumnMappings.Add("EntryDatetime", "EntryDatetime");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");

            con.Open();
            objbulk.WriteToServer(customEnumDataTable);
            con.Close();
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.CustomEnums.Any(x => !x.IsDeleted && x.ThesaurusEntryId == thesaurusId);
        }

        public int UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            int entriesUpdated = 0;
            foreach(CustomEnum customEnum in GetAllByThesaurusId(oldThesaurus))
            {
                customEnum.ReplaceThesauruses(oldThesaurus, newThesaurus);
                ++entriesUpdated;
            }
            context.SaveChanges();

            return entriesUpdated;
        }

        private List<CustomEnum> GetAllByThesaurusId(int thesaurusId)
        {
            return context.CustomEnums.Where(x => !x.IsDeleted && x.ThesaurusEntryId == thesaurusId).ToList();
        }
    }
}
