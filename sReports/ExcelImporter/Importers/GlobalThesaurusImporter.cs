using DocumentFormat.OpenXml.Packaging;
using ExcelImporter.Classes;
using ExcelImporter.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    public class GlobalThesaurusImporter : ExcelSaxImporter<ThesaurusEntry>
    {
        private readonly IGlobalThesaurusUserDAL globalThesaurusUserDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IThesaurusTranslationDAL translationDAL;
        private readonly IO4CodeableConceptDAL o4codeableConceptDAL;
        private readonly IAdministrativeDataDAL administrativeDataDAL;

        private readonly Dictionary<string, string> languages;

        public GlobalThesaurusImporter(IThesaurusDAL thesaurusDAL, IGlobalThesaurusUserDAL globalThesaurusUserDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, IAdministrativeDataDAL administrativeDataDAL, string fileName, string sheetName) : base(fileName, sheetName)
        {
            this.globalThesaurusUserDAL = globalThesaurusUserDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.administrativeDataDAL = administrativeDataDAL;
            this.languages = SetLanguages();
        }

        private Dictionary<string, string> SetLanguages()
        {
            return new Dictionary<string, string>()
            {
                {"English", "en" },
                {"German", "de" }
            };
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (thesaurusDAL.GetAllCount() == 0)
            {
                List<ThesaurusEntry> thesaurusEntries = ImportFromExcel();
                InsertDataIntoDatabase(thesaurusEntries);
            }
        }

        protected override List<ThesaurusEntry> ImportFromExcel()
        {
            List<RowInfo> dataRows = ImportRowsFromExcel();
            List<ThesaurusEntry> thesaurusEntries = GetThesauruses(dataRows);

            return thesaurusEntries;
        }

        protected override void InsertDataIntoDatabase(List<ThesaurusEntry> thesauruses)
        {
            thesaurusDAL.InsertMany(thesauruses);
            var bulkedThesauruses = thesaurusDAL.GetBulkInserted(thesauruses.Count());
            translationDAL.InsertMany(thesauruses, bulkedThesauruses);
            o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertManyVersions(thesauruses, bulkedThesauruses);
        }

        private List<ThesaurusEntry> GetThesauruses(List<RowInfo> dataRows)
        {
            List<ThesaurusEntry> thesaurusEntries = new List<ThesaurusEntry>();

            var groupByPrefTerms = dataRows.GroupBy(row => row.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.PreferredTerm)));

            foreach (var groupByPrefTerm in groupByPrefTerms)
            {
                var thesaurus = GetThesaurus(groupByPrefTerm);
                thesaurusEntries.Add(thesaurus);
            }

            return thesaurusEntries;
        }

        private ThesaurusEntry GetThesaurus(IGrouping<string, RowInfo> groupByPrefTerm)
        {
            ThesaurusEntry thesaurusEntry = new ThesaurusEntry();

            RowInfo rowInfo = groupByPrefTerm.FirstOrDefault();
            SetInitialData(rowInfo, thesaurusEntry);
            SetTranslations(rowInfo, thesaurusEntry);
            SetCoding(groupByPrefTerm, thesaurusEntry);
            SetAdministrativeData(rowInfo, thesaurusEntry);

            return thesaurusEntry;
        }

        private void SetInitialData(RowInfo rowInfo, ThesaurusEntry thesaurusEntry)
        {
            thesaurusEntry.State = ThesaurusState.Curated;
            thesaurusEntry.PreferredLanguage = GetLanguage(rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.Language)));
        }

        private void SetTranslations(RowInfo rowInfo, ThesaurusEntry thesaurusEntry)
        {
            ThesaurusEntryTranslation translation = new ThesaurusEntryTranslation
            {
                PreferredTerm = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.PreferredTerm)),
                Definition = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.Definition)),
                Language = thesaurusEntry.PreferredLanguage,
                Synonyms = ParseCollectionTypeFromExcel(rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.Synonyms))),
                Abbreviations = ParseCollectionTypeFromExcel(rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.Abbreviations)))
            };

            thesaurusEntry.Translations = new List<ThesaurusEntryTranslation>() { translation };
        }

        private void SetCoding(IGrouping<string, RowInfo> groupByPrefTerm, ThesaurusEntry thesaurusEntry)
        {
            List<O4CodeableConcept> codes = new List<O4CodeableConcept>();
            foreach(RowInfo rowInfo in groupByPrefTerm)
            {
                var code = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.CCCode));
                if (!string.IsNullOrWhiteSpace(code))
                {
                    O4CodeableConcept codeObject = new O4CodeableConcept()
                    {
                        Code = code,
                        Value = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.CCValue)),
                        Link = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.CCLink)),
                        VersionPublishDate = DateTime.Now,
                        EntryDateTime = DateTime.Now,
                        CodeSystemId = 1
                    };
                    codes.Add(codeObject);
                }
            }

            thesaurusEntry.Codes = codes;
        }

        private void SetAdministrativeData(RowInfo rowInfo, ThesaurusEntry thesaurusEntry)
        {
            string verifiedByMail = rowInfo.GetCellValue(GetColumnAddress(GlobalThesaurusConstants.VerifiedByMail));
            if (!string.IsNullOrWhiteSpace(verifiedByMail))
            {
                var user = globalThesaurusUserDAL.GetByEmail(verifiedByMail);
                if(user != null)
                {
                    thesaurusEntry.AdministrativeData = new AdministrativeData(new sReportsV2.Common.Entities.User.UserData() { Id = user.GlobalThesaurusUserId}, ThesaurusState.Curated);
                }
            }
        }

        private string GetLanguage(string key)
        {
            return languages.TryGetValue(key, out string language) ? language : "en";
        }
    }
}
