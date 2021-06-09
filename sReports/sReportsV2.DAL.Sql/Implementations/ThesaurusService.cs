using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Dynamic.Core;
using System.Diagnostics;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.DAL.Sql.Interfaces;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class ThesaurusService : IThesaurusService
    {
        public void InsertMany(List<ThesaurusEntry> thesauruses)
        {
            DataTable thesaurusesTable = new DataTable();
            thesaurusesTable.Columns.Add(new DataColumn("UmlsCode", typeof(string)));
            thesaurusesTable.Columns.Add(new DataColumn("UmlsName", typeof(string)));
            thesaurusesTable.Columns.Add(new DataColumn("State", typeof(ThesaurusState)));
            thesaurusesTable.Columns.Add(new DataColumn("IsDeleted", typeof(bool)));
            thesaurusesTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTime)));
            thesaurusesTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTime)));
            thesaurusesTable.Columns.Add(new DataColumn("UmlsDefinition", typeof(string)));


            foreach (var thesaurus in thesauruses) 
            {
                DataRow thesaurusRow = thesaurusesTable.NewRow();
                thesaurusRow["State"] = thesaurus.State;
                thesaurusRow["IsDeleted"] = false;
                thesaurusRow["EntryDatetime"] = DateTime.Now;
                thesaurusRow["LastUpdate"] = DateTime.Now;
                thesaurusesTable.Rows.Add(thesaurusRow);
            }


            string connection = ConfigurationManager.ConnectionStrings["Sql"].ConnectionString;
            SqlConnection con = new SqlConnection(connection);
           
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;
            objbulk.DestinationTableName = "ThesaurusEntries";
            objbulk.ColumnMappings.Add("UmlsCode", "UmlsCode");
            objbulk.ColumnMappings.Add("UmlsName", "UmlsName");
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
            objbulk.ColumnMappings.Add("EntryDateTime", "EntryDatetime");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");
            objbulk.ColumnMappings.Add("UmlsDefinition", "UmlsDefinition");

            con.Open();
            objbulk.WriteToServer(thesaurusesTable);
            con.Close();

            
        }

        public Dictionary<string, int> GetLastBulkInserted(int size) 
        {
            using(var context = new SReportsContext()) 
            {
                return context.Thesauruses.OrderByDescending(x => x.EntryDatetime).Take(size).Select(x => new ThesaurusEntryBulkInfo()
                {
                    Id = x.Id
                }).ToDictionary(t => t.UmlsCode, t => t.Id);
            }
        }

        public void Insert(ThesaurusEntry thesaurus)
        {
            thesaurus.EntryDatetime = DateTime.Now;
            thesaurus.LastUpdate = DateTime.Now;
            using (var context = new SReportsContext())
            {
               context.Thesauruses.Add(thesaurus);
                context.SaveChanges();
            }
        }

        public string InsertOrUpdate(ThesaurusEntry thesaurus)
        {
            if (thesaurus.Id == null)
            {
                thesaurus.EntryDatetime = DateTime.Now;
            }

            thesaurus.LastUpdate = DateTime.Now;

            using (var context = new SReportsContext())
            {
                context.Thesauruses.AddOrUpdate(thesaurus);
                context.SaveChanges();
            }

            return thesaurus.O40MTId;
        }

        public ThesaurusEntry GetById(int id)
        {
            using (var context = new SReportsContext())
            {
                return context.Thesauruses.Include(t => t.Translations)
                    //.Include(t => t.Translations.Select(tr => tr.SimilarTerms))
                    .FirstOrDefault(x => x.Id == id);
            }
        }

        public int GetAllCount()
        {
            int result = 0;
            using(var context = new SReportsContext())
            {
                result = context.Thesauruses.Count();
            }

            return result;
        }

        public List<string> GetAll(ThesaurusFilter filter)
        {
            string term = filter.Terms.FirstOrDefault();
            using (var context = new SReportsContext())
            {
                Stopwatch s = new Stopwatch();
                
                s.Start();
                var translations = context.ThesaurusEntryTranslations
                   .Where(x => x.Language == "en")
                   //.Include(t => t.SimilarTerms)
                   .Where(x =>
                   //x.SimilarTerms.Any(z => z.Name.Contains(term))  || 
                    x.PreferredTerm.Contains(term) )
                   //.OrderBy(x => x.Id)
                   //.Skip(() => 0)
                   //.Take(10)
                   //.Select(x => new
                   //{
                   //    PreferredTerm = x.PreferredTerm,
                   //    SimilarTerms = x.SimilarTerms
                   //})
                   .ToList();
                s.Stop();

                List<string> result = new List<string>();
                foreach (var translation in translations) 
                {
                    //result.AddRange(translation.SimilarTerms.Select(x => x.Name).ToList());
                    if (!string.IsNullOrWhiteSpace(translation.PreferredTerm)) 
                    {
                        result.Add(translation.PreferredTerm);
                    }
                }


                return result.Select( x => x.ToUpper()).Distinct().ToList() ;
            }
        }
    }
}
