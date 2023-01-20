using ExcelImporter.Classes;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    public class CountryCodeImporter : ExcelSaxImporter<O4CodeableConcept>
    {
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeSystemDAL codeSystemDAL;
        private readonly ICustomEnumDAL customEnumDAL;
        private readonly IThesaurusTranslationDAL translationDAL;
        private readonly IO4CodeableConceptDAL o4codeableConceptDAL;
        private readonly IAdministrativeDataDAL administrativeDataDAL;

        private const string Code = "Code";
        private const string Value = "Display";
        private const string DefaultLanguage = "en";
        private const string CountryCodingSystem = "urn:iso:std:iso:3166";

        private int countryCodeSystemId;
        private readonly int createdById;
        private readonly int organizationId;

        public CountryCodeImporter(string fileName, string sheetName, IThesaurusDAL thesaurusDAL, ICustomEnumDAL customEnumDAL, ICodeSystemDAL codeSystemDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, IAdministrativeDataDAL administrativeDataDAL, int createdById, int organizationId) : base(fileName, sheetName)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.customEnumDAL = customEnumDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.administrativeDataDAL = administrativeDataDAL;

            this.createdById = createdById;
            this.organizationId = organizationId;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (!customEnumDAL.CustomEnumExist(sReportsV2.Common.Enums.CustomEnumType.Country))
            {
                SetCountryCodeSystem();
                List<O4CodeableConcept> countryCodes = ImportFromExcel();
                InsertDataIntoDatabase(countryCodes);
            }
        }

        protected override List<O4CodeableConcept> ImportFromExcel()
        {
            return ImportRowsFromExcel().Select(row => GetCode(row)).ToList();
        }

        protected override void InsertDataIntoDatabase(List<O4CodeableConcept> entries)
        {
            List<ThesaurusEntry> thesauruses = GetThesauruses(entries);

            thesaurusDAL.InsertMany(thesauruses);
            var bulkedThesauruses = thesaurusDAL.GetLastBulkInserted(thesauruses.Count());
            translationDAL.InsertMany(thesauruses, bulkedThesauruses);
            o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertManyVersions(thesauruses, bulkedThesauruses);
            customEnumDAL.InsertMany(bulkedThesauruses, organizationId, sReportsV2.Common.Enums.CustomEnumType.Country);
        }

        private void SetCountryCodeSystem()
        {
            sReportsV2.Domain.Sql.Entities.CodeSystem.CodeSystem codeSystem = codeSystemDAL.GetByValue(CountryCodingSystem);
            if (codeSystem == null)
            {
                codeSystem = new sReportsV2.Domain.Sql.Entities.CodeSystem.CodeSystem()
                {
                    Value = CountryCodingSystem,
                    Label = "ISO 3166 Codes for the representation of names of countries and their subdivisions"
                };
                codeSystemDAL.InsertOrUpdate(codeSystem);
            }
            countryCodeSystemId = codeSystem.CodeSystemId;
        }

        private O4CodeableConcept GetCode(RowInfo dataRow)
        {
            return new O4CodeableConcept()
            {
                Code = dataRow.GetCellValue(GetColumnAddress(Code)),
                Value = dataRow.GetCellValue(GetColumnAddress(Value)),
                CodeSystemId = countryCodeSystemId, 
                VersionPublishDate = DateTime.Now,
                EntryDateTime = DateTime.Now
            };
        }

        private List<ThesaurusEntry> GetThesauruses(List<O4CodeableConcept> codes)
        {
            List<ThesaurusEntry> thesaurusEntries = new List<ThesaurusEntry>();
            foreach (IGrouping<string, O4CodeableConcept> codesByCountry in codes.GroupBy(x => x.Value))
            {
                thesaurusEntries.Add(GetThesaurus(codesByCountry));
            }
            return thesaurusEntries;
        }

        private ThesaurusEntry GetThesaurus(IGrouping<string, O4CodeableConcept> codesByCountry)
        {
            List<O4CodeableConcept> countryCodes = codesByCountry.ToList();
            string country = codesByCountry.Key;
            return new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = DefaultLanguage,
                            PreferredTerm = country,
                            Definition = country,
                            Abbreviations = countryCodes.Select(c => c.Code).ToList()
                        }
                    },
                Codes = countryCodes,
                State = sReportsV2.Common.Enums.ThesaurusState.Draft,
                CreatedById = createdById,
                AdministrativeData = new AdministrativeData(new sReportsV2.Common.Entities.User.UserData() { Id = createdById, ActiveOrganization = organizationId }, sReportsV2.Common.Enums.ThesaurusState.Draft)
            };
        }
    }
}