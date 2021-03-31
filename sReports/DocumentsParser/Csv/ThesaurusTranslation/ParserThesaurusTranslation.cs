using CsvHelper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsParser.Csv.ThesaurusTranslation
{
    public static class ParserThesaurusTranslation
    {
        public static void ParseAndUpdateThesaurus(UserData userData)
        {
            List<ThesaurusData> inserted = new List<ThesaurusData>();
            ThesaurusEntryService service = new ThesaurusEntryService();
            using (var reader = new StreamReader("C:\\Users\\danil\\Downloads\\NOVEL CORONAVIRUS (nCoV) - Sheet1.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<ThesaurusData>().ToList();

                foreach(ThesaurusData data in records.Where(x => !string.IsNullOrWhiteSpace(x.ThesaurusId)))
                {
                    ThesaurusEntry entry = service.GetByO4MtIdId(data.ThesaurusId);
                    if(entry != null)
                    {
                        entry.SetPrefferedTermAndDescriptionForLang("fr", data.FrancePreferredTerm, data.DefinitionFrance);
                        entry.SetPrefferedTermAndDescriptionForLang("de", data.GermanPreferredTerm, data.DefinitionGerman);
                        entry.SetPrefferedTermAndDescriptionForLang("sr", data.SrpskiLtPreferredTerm, data.DefinitionSerbianLt);
                        entry.SetPrefferedTermAndDescriptionForLang("sr-Cyrl-RS", data.SrpskiCrPreferredTerm, data.DefinitionSerbianCr);
                        entry.SetPrefferedTermAndDescriptionForLang("ru", data.RussianPreferredTerm, data.DefinitionRussian);
                        entry.SetPrefferedTermAndDescriptionForLang("es", data.SpanishPrefferedTerm, data.DefinitionSpanish);
                        entry.SetPrefferedTermAndDescriptionForLang("pt", data.PortuguesePrefferedTerm, data.DefinitionPortuguese);

                        entry.UmlsCode = data.UMLS;

                        var translationEn = entry.Translations.FirstOrDefault(x => x.Language == "en");
                        if (translationEn != null)
                        {
                            if (!string.IsNullOrWhiteSpace(translationEn.Definition) && translationEn.Definition.Equals("."))
                            {
                                translationEn.Definition = "";
                            }
                        }
                        if(!inserted.Any(x => x.ThesaurusId.Equals(data.ThesaurusId)))
                        {
                            service.InsertOrUpdate(entry, userData);
                            inserted.Add(data);
                        }

                    }
                }
            }
        }
    }
}
