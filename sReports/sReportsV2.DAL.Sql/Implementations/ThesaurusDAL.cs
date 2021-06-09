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

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusDAL : IThesaurusDAL
    {
        private SReportsContext context;
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
                .FirstOrDefault(x => x.Id == id & !x.IsDeleted);
        }

        public O4CodeableConcept GetCodeById(int id)
        {
            return context.O4CodeableConcept
                .FirstOrDefault(x => x.Id == id);
        }

        public List<ThesaurusEntry> GetFiltered(GlobalThesaurusFilter filterDataIn)
        {
            return this.GetFilteredQuery(filterDataIn)
                      .OrderBy(x => x.Id)
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
            IQueryable<ThesaurusEntry> result = context.Thesauruses.Include(x => x.Translations)
                                                                   .Include(x => x.Codes);
            if (filterDataIn != null) 
            {
                if (!string.IsNullOrWhiteSpace(filterDataIn.Term))
                {
                    result = result.Where(x => x.Translations.Any(t => t.PreferredTerm.Contains(filterDataIn.Term) && (filterDataIn.Language == null || t.Language == filterDataIn.Language)));
                }
            }

            return result;
        }

        public int InsertOrUpdate(ThesaurusEntry thesaurus)
        {
            if (thesaurus.Id == 0)
            {
                thesaurus.Codes = new List<O4CodeableConcept>();
                thesaurus.EntryDatetime = DateTime.Now;
                context.Thesauruses.Add(thesaurus);
            }
            else 
            {
                ThesaurusEntry dbThesaurus = this.GetById(thesaurus.Id);
                MapThesaurusValues(dbThesaurus, thesaurus);
            }



            context.SaveChanges();

            return thesaurus.Id;
        }

        public int InsertOrUpdateCode(O4CodeableConcept code, int id)
        {
            try
            {
                if (code.Id == 0)
                {
                    code.ThesaurusEntryId = id;
                    code.EntryDateTime = DateTime.Now;
                    ThesaurusEntry t = context.Thesauruses.Include(x => x.Codes).FirstOrDefault(x => x.Id == id);
                    t.Codes.Add(code);
                }
                else
                {
                    O4CodeableConcept dbCode = this.GetCodeById(code.Id);
                    MapCodeValues(dbCode, code);
                }

                context.SaveChanges();
            }
            catch (Exception e) 
            {
                
            }

            return id;
        }

        private void MapThesaurusValues(ThesaurusEntry dbThesaurus, ThesaurusEntry thesaurus) 
        {
            dbThesaurus.State = thesaurus.State;
            dbThesaurus.Translations = MapTranslationsValues(dbThesaurus.Translations, thesaurus.Translations);
        }

        private void MapCodes(List<O4CodeableConcept> dbCodes, List<O4CodeableConcept> codes)
        {
            RemoveNonExistingCodes(dbCodes, codes);
            AddNewOrUpdateOldCodes(dbCodes, codes);
        }

        public void AddNewOrUpdateOldCodes(List<O4CodeableConcept> dbCodes, List<O4CodeableConcept> codes)
        {
            foreach (var code in codes)
            {
                if (code.Id == 0)
                {
                    dbCodes.Add(code);
                }
                else
                {
                    MapCodeValues(dbCodes.FirstOrDefault(x => x.Id == code.Id), code);
                }
            }
        }

        private void RemoveNonExistingCodes(List<O4CodeableConcept> dbCodes, List<O4CodeableConcept> codes) 
        {
            for (int i =0; i< dbCodes.Count(); i++)
            {
                if (!codes.Any(x => x.Id == dbCodes[i].Id))
                {
                    context.O4CodeableConcept.Remove(dbCodes[i]);
                    context.SaveChanges();
                }
            }
        }

        private List<ThesaurusEntryTranslation> MapTranslationsValues(List<ThesaurusEntryTranslation> dbTranslations, List<ThesaurusEntryTranslation> translations) 
        {
            foreach (var translation in dbTranslations) 
            {
                var t = translations.FirstOrDefault(x => x.Language == translation.Language);
                translation.PreferredTerm = t.PreferredTerm;
                translation.Definition = t.Definition;
                translation.Synonyms = t.Synonyms;
                translation.Abbreviations = t.Abbreviations;
            }

            foreach (var t in translations.Where(x => !dbTranslations.Select(c => c.Language).Contains(x.Language)).ToList()) 
            {
                dbTranslations.Add(t);
            }
            return dbTranslations;
        }

        private void MapCodeValues(O4CodeableConcept dbCode, O4CodeableConcept code)
        {
            dbCode.Version = code.Version;
            dbCode.Code = code.Code;
            dbCode.Value = code.Value;
            dbCode.Link = code.Link;
            dbCode.CodeSystemId = code.CodeSystemId;
        }

        public void DeleteCode(int id)
        {
            O4CodeableConcept code = context.O4CodeableConcept.FirstOrDefault(x => x.Id == id);
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


            foreach (var thesaurus in thesauruses)
            {
                DataRow thesaurusRow = thesaurusesTable.NewRow();
                thesaurusRow["State"] = thesaurus.State;
                thesaurusRow["IsDeleted"] = false;
                thesaurusRow["EntryDatetime"] = DateTime.Now;
                thesaurusRow["LastUpdate"] = DateTime.Now;
                thesaurusesTable.Rows.Add(thesaurusRow);
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

            con.Open();
            objbulk.WriteToServer(thesaurusesTable);
            con.Close();


        }

        public List<int> GetLastBulkInserted(int size)
        {
             return context.Thesauruses.OrderByDescending(x => x.EntryDatetime).Take(size).Select(x => x.Id).ToList();
            
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
                result = result.Where(x => (filterData.Id == 0 || x.Id.Equals(filterData.Id))
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
                        .Where(x => x.Translations.Any(y => y.Abbreviations.Any(a => a.Contains(filterData.Abbreviation))));
                }


                if (!string.IsNullOrEmpty(filterData.Synonym))
                {
                    result = result
                        .Where(x => x.Translations.Any(y => y.Synonyms.Any(a => a.Contains(filterData.Synonym))));
                }
            }

            return result;
        }

        public List<ThesaurusEntry> GetAll(ThesaurusEntryFilterData filterData)
        {
            return GetThesaurusEntriesFiltered(filterData)
                .OrderByDescending(x => x.Id)
                .Skip((filterData.Page - 1) * filterData.PageSize)
                .Take(filterData.PageSize)
                .ToList();
        }

        public string InsertOrUpdate(ThesaurusEntry thesaurusEntry, UserData user)
        {
            if (thesaurusEntry.Id != 0)
            {
                UpdateThesaurusEntry(thesaurusEntry, user);
            }
            else
            {
                InsertThesaurusEntry(thesaurusEntry, user);
            }

            return thesaurusEntry.Id.ToString();
        }

        private void InsertThesaurusEntry(ThesaurusEntry thesaurusEntry, UserData user) 
        {
            thesaurusEntry.EntryDatetime = DateTime.Now;
            thesaurusEntry.AdministrativeData = new AdministrativeData(user, thesaurusEntry.State);

            context.Thesauruses.Add(thesaurusEntry);
            context.SaveChanges();
        }

        private void UpdateThesaurusEntry(ThesaurusEntry thesaurusEntry, UserData user)
        {
            ThesaurusEntry entry = this.GetById(thesaurusEntry.Id);
            MapThesaurusValues(entry, thesaurusEntry);
            MapCodes(entry.Codes, thesaurusEntry.Codes);
            entry.AdministrativeData = entry.AdministrativeData == null ? new AdministrativeData(user, ThesaurusState.Draft) : entry.AdministrativeData;
            entry.AdministrativeData.UpdateVersionHistory(user, thesaurusEntry.State);
            context.SaveChanges();

        }

        public bool ExistsThesaurusEntry(int id)
        {
            return context.Thesauruses.Any(x => x.Id == id);
        }

        public void Delete(int id)
        {
            var entity = this.GetById(id);
            entity.IsDeleted = true;
            context.SaveChanges();

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
                .Where(x => !x.IsDeleted && x.Id != filter.Id && x.State == ThesaurusState.Production)
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
                .Where(x => thesaurusList.Contains(x.Id) && !x.IsDeleted)
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
                .OrderBy(x => x.Id)
                .Skip(page).Take(10)
                .ToList()
                .Select(m => m.GetPreferredTermByActiveLanguage(language))
                .ToList();
        }
    }
}
