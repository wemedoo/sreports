using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using sReportsV2.Common.Enums;
using System.Globalization;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Constants;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusDAL : IThesaurusDAL
    {
        private readonly SReportsContext context;
        public ThesaurusDAL(SReportsContext context)
        {
            this.context = context;
        }

        public ThesaurusEntry GetById(int id)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .Include("Codes.System")
                .FirstOrDefault(x => x.ThesaurusEntryId == id & !x.IsDeleted);
        }

        public O4CodeableConcept GetCodeById(int id)
        {
            return context.O4CodeableConcept
                .FirstOrDefault(x => x.O4CodeableConceptId == id);
        }

        public List<ThesaurusEntry> GetFiltered(GlobalThesaurusFilter filterDataIn)
        {
            return this.GetFilteredQuery(filterDataIn)
                      .OrderBy(x => x.ThesaurusEntryId)
                      .Skip((filterDataIn.Page - 1) * filterDataIn.PageSize)
                      .Take(filterDataIn.PageSize)
                      .ToList(); 
        }

        public int GetFilteredCount(GlobalThesaurusFilter filterDataIn)
        {
            return this.GetFilteredQuery(filterDataIn).Count();
        }

        public IQueryable<ThesaurusEntry> GetFilteredQuery(GlobalThesaurusFilter filterDataIn)
        {
            IQueryable<ThesaurusEntry> result = context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.Codes)
                .Where(x => !x.IsDeleted);

            if (filterDataIn != null) 
            {
                if (!string.IsNullOrWhiteSpace(filterDataIn.Term))
                {
                    if(!string.IsNullOrWhiteSpace(filterDataIn.TermIndicator) && filterDataIn.TermIndicator.Equals("exact"))
                    {
                        result = result.Where(x => x.Translations.Any(t => t.PreferredTerm.Equals(filterDataIn.Term) && (filterDataIn.Language == null || t.Language == filterDataIn.Language)));
                    } 
                    else
                    {
                        result = result.Where(x => x.Translations.Any(t => t.PreferredTerm.Contains(filterDataIn.Term) && (filterDataIn.Language == null || t.Language == filterDataIn.Language)));
                    }
                }
            }

            return result;
        }

        public void InsertOrUpdate(ThesaurusEntry thesaurus)
        {
            if (thesaurus.ThesaurusEntryId == 0)
            {
                context.Thesauruses.Add(thesaurus);
            }
            else
            {
                thesaurus.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public void DeleteCode(int id)
        {
            O4CodeableConcept code = context.O4CodeableConcept.FirstOrDefault(x => x.O4CodeableConceptId == id);
            code.IsDeleted = true;
            context.SaveChanges();
        }

        public int GetAllCount()
        {
            return context.Thesauruses.Where(x => !x.IsDeleted).Count();
        }
        public int GetAllEntriesCount(ThesaurusEntryFilterData filterData)
        {
            return GetThesaurusEntriesFiltered(filterData).Count();
        }

        public long GetUmlsEntriesCount()
        {
            return context.Thesauruses
                .Where(x => x.Codes.Any(c => c.System.SAB == "MTH"))
                .Count();
        }

        //Import thesaurus from UMLS section
        public void InsertMany(List<ThesaurusEntry> thesauruses)
        {
            DataTable thesaurusesTable = new DataTable();
            thesaurusesTable.Columns.Add(new DataColumn("State", typeof(ThesaurusState)));
            thesaurusesTable.Columns.Add(new DataColumn("IsDeleted", typeof(bool)));
            thesaurusesTable.Columns.Add(new DataColumn("EntryDatetime", typeof(DateTime)));
            thesaurusesTable.Columns.Add(new DataColumn("LastUpdate", typeof(DateTime)));
            thesaurusesTable.Columns.Add(new DataColumn("PreferredLanguage", typeof(string)));
            thesaurusesTable.Columns.Add(new DataColumn("AdministrativeDataId", typeof(int)));

            int i = 1;
            foreach (var thesaurus in thesauruses)
            {
                DataRow thesaurusRow = thesaurusesTable.NewRow();
                thesaurusRow["State"] = thesaurus.State;
                thesaurusRow["IsDeleted"] = false;
                thesaurusRow["EntryDatetime"] = thesaurus.EntryDatetime;
                thesaurusRow["LastUpdate"] = DBNull.Value;
                thesaurusRow["PreferredLanguage"] = thesaurus.PreferredLanguage;
                thesaurusRow["AdministrativeDataId"] = i;
                thesaurusesTable.Rows.Add(thesaurusRow);
                i++;
            }


            string connection = ConfigurationManager.AppSettings["Sql"];
            SqlConnection con = new SqlConnection(connection);

            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            objbulk.BulkCopyTimeout = 0;
            objbulk.DestinationTableName = "ThesaurusEntries";
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("IsDeleted", "IsDeleted");
            objbulk.ColumnMappings.Add("EntryDateTime", "EntryDatetime");
            objbulk.ColumnMappings.Add("LastUpdate", "LastUpdate");
            objbulk.ColumnMappings.Add("PreferredLanguage", "PreferredLanguage");
            objbulk.ColumnMappings.Add("AdministrativeDataId", "AdministrativeDataId");

            con.Open();
            objbulk.WriteToServer(thesaurusesTable);
            con.Close();


        }

        public List<int> GetLastBulkInserted(int size)
        {
             return context.Thesauruses.OrderByDescending(x => x.EntryDatetime).Take(size).Select(x => x.ThesaurusEntryId).ToList();
            
        }

        public List<int> GetBulkInserted(int size)
        {
            return context.Thesauruses.OrderBy(x => x.ThesaurusEntryId).Take(size).Select(x => x.ThesaurusEntryId).ToList();

        }
        //End import thesaurus from UMLS section

        private IQueryable<ThesaurusEntry> GetThesaurusEntriesFiltered(ThesaurusEntryFilterData filterData)
        {
            IQueryable<ThesaurusEntry> result = context.Thesauruses
                .Include(x => x.Translations)
                //.Include(x => x.AdministrativeData)
                //.Include(x => x.Codes)
                .Where(x => !x.IsDeleted);

            if (filterData != null)
            {
                result = result.Where(x => (filterData.Id == 0 || x.ThesaurusEntryId.Equals(filterData.Id))
                        && (filterData.State == null || x.State == filterData.State)
                    );
                if (!string.IsNullOrEmpty(filterData.PreferredTerm))
                {
                    result = result
                        .Where(x => x.Translations.Any(y => y.PreferredTerm.Contains(filterData.PreferredTerm)));
                }

                if (!string.IsNullOrEmpty(filterData.Abbreviation))
                {
                    result = result
                        .Where(x => x.Translations.Any(y => !string.IsNullOrEmpty(y.AbbreviationsString) && y.AbbreviationsString.Contains(filterData.Abbreviation)));
                }

                if (!string.IsNullOrEmpty(filterData.Synonym))
                {
                    result = result
                        .Where(x => x.Translations.Any(y => !string.IsNullOrEmpty(y.SynonymsString) && y.SynonymsString.Contains(filterData.Synonym)));
                }
            }

            return result;
        }

        public List<ThesaurusEntry> GetAll(ThesaurusEntryFilterData filterData)
        {
            IQueryable<ThesaurusEntry> result = GetThesaurusEntriesFiltered(filterData);

            if (filterData.ColumnName != null)
                result = SortByField(result, filterData);
            else
                result = result.OrderByDescending(x => x.ThesaurusEntryId)
                    .Skip((filterData.Page - 1) * filterData.PageSize)
                    .Take(filterData.PageSize);

            return result.ToList();
        }

        public bool ExistsThesaurusEntry(int id)
        {
            return context.Thesauruses.Any(x => x.ThesaurusEntryId == id);
        }

        public void Delete(int id)
        {
            var fromDb = GetById(id);
            if(fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public List<ThesaurusEntry> GetAllSimilar(ThesaurusReviewFilterData filter, string preferredTerm, string language)
        {
            return !string.IsNullOrWhiteSpace(preferredTerm) ? this.GetAllSimilarQuery(filter, preferredTerm, language)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList() : new List<ThesaurusEntry>();
        }

        public long GetAllSimilarCount(ThesaurusReviewFilterData filter, string preferredTerm, string language)
        {
            return !string.IsNullOrWhiteSpace(preferredTerm) ? this.GetAllSimilarQuery(filter, preferredTerm, language).Count() : 0;
        }

        private IQueryable<ThesaurusEntry> GetAllSimilarQuery(ThesaurusReviewFilterData filter, string preferredTerm, string language)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .Where(x => !x.IsDeleted && x.ThesaurusEntryId != filter.Id && x.State == ThesaurusState.Production)
                .Where(x => x.Translations.Any(y => y.Language == language && y.PreferredTerm.Contains(preferredTerm)));

        }

        public void UpdateState(int thesaurusId, ThesaurusState state)
        {
            ThesaurusEntry thesaurus = this.GetById(thesaurusId);
            thesaurus.State = state;
            context.SaveChanges();
            
        }

        public List<ThesaurusEntry> GetByIdsList(List<int> thesaurusList)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .Where(x => thesaurusList.Contains(x.ThesaurusEntryId) && !x.IsDeleted)
                .ToList();
        }

        public List<string> GetAll(string language, string searchValue, int page)
        {
            return context.Thesauruses
                .Include(x => x.Translations)
                .Include(x => x.AdministrativeData)
                .Include(x => x.Codes)
                .Include(x => x.AdministrativeData.VersionHistory)
                .Where(x => !x.IsDeleted)
                .Where(z => z.Translations
                .Any(y => y.Language.Equals(language) && y.PreferredTerm.Contains(searchValue)))
                .OrderBy(x => x.ThesaurusEntryId)
                .Skip(page).Take(10)
                .ToList()
                .Select(m => m.GetPreferredTermByActiveLanguage(language))
                .ToList();
        }

        public int GetIdByPreferredTerm(string prefferedTerm)
        {
            return context.ThesaurusEntryTranslations.Where(x => x.PreferredTerm == prefferedTerm)
                .Select(x => x.ThesaurusEntryId).FirstOrDefault();
        }

        private IQueryable<ThesaurusEntry> SortByField(IQueryable<ThesaurusEntry> result, ThesaurusEntryFilterData filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.PreferredTerm:
                    if (filterData.IsAscending)
                        return result.ToList().OrderBy(x => x.GetPreferredTermByTranslationOrDefault("en", filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.ToList().OrderByDescending(x => x.GetPreferredTermByTranslationOrDefault("en", filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.Definition:
                    if (filterData.IsAscending)
                        return result.ToList().OrderBy(x => x.GetDefinitionByTranslationOrDefault("en", filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.ToList().OrderByDescending(x => x.GetDefinitionByTranslationOrDefault("en", filterData.ActiveLanguage))
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.State:
                    string draft = filterData.ThesaurusStates[(int)ThesaurusState.Draft];
                    string production = filterData.ThesaurusStates[(int)ThesaurusState.Production];
                    string deprecated = filterData.ThesaurusStates[(int)ThesaurusState.Deprecated];
                    string disabled = filterData.ThesaurusStates[(int)ThesaurusState.Disabled];
                    string curated = filterData.ThesaurusStates[(int)ThesaurusState.Curated];
                    string uncurated = filterData.ThesaurusStates[(int)ThesaurusState.Uncurated];
                    string metadataIncomplete = filterData.ThesaurusStates[(int)ThesaurusState.MetadataIncomplete];
                    string requiresDiscussion = filterData.ThesaurusStates[(int)ThesaurusState.RequiresDiscussion];
                    string pendingFinalVetting = filterData.ThesaurusStates[(int)ThesaurusState.PendingFinalVetting];
                    string readyForRelease = filterData.ThesaurusStates[(int)ThesaurusState.ReadyForRelease];
                    string toBeReplacedWithExternalOntologyTerm = filterData.ThesaurusStates[(int)ThesaurusState.ToBeReplacedWithExternalOntologyTerm];
                    string organizationalTerm = filterData.ThesaurusStates[(int)ThesaurusState.OrganizationalTerm];
                    string exampleToBeEventuallyRemoved = filterData.ThesaurusStates[(int)ThesaurusState.ExampleToBeEventuallyRemoved];
                    
                    if (filterData.IsAscending)
                        return result.OrderBy(x => (int)x.State == 0 ? draft : (int)x.State == 1 ? production : (int)x.State == 2 ? deprecated : (int)x.State == 3 ? disabled :
                                (int)x.State == 4 ? curated : (int)x.State == 5 ? uncurated : (int)x.State == 6 ? metadataIncomplete : (int)x.State == 7 ? requiresDiscussion :
                                (int)x.State == 8 ? pendingFinalVetting : (int)x.State == 9 ? readyForRelease :
                                (int)x.State == 10 ? toBeReplacedWithExternalOntologyTerm : (int)x.State == 11 ? organizationalTerm : exampleToBeEventuallyRemoved)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => (int)x.State == 0 ? draft : (int)x.State == 1 ? production : (int)x.State == 2 ? deprecated : (int)x.State == 3 ? disabled :
                                (int)x.State == 4 ? curated : (int)x.State == 5 ? uncurated : (int)x.State == 6 ? metadataIncomplete : (int)x.State == 7 ? requiresDiscussion :
                                (int)x.State == 8 ? pendingFinalVetting : (int)x.State == 9 ? readyForRelease :
                                (int)x.State == 10 ? toBeReplacedWithExternalOntologyTerm : (int)x.State == 11 ? organizationalTerm : exampleToBeEventuallyRemoved)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                                .Skip((filterData.Page - 1) * filterData.PageSize)
                                .Take(filterData.PageSize);
            }
        }
    }
}
