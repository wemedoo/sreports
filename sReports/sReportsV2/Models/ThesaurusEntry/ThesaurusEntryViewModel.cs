using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.ThesaurusEntry
{
    public class ThesaurusEntryViewModel
    {
        public string Id { get; set; }
        public string O40MTId { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public string UmlsDefinitions { get; set; }
        public DateTime? LastUpdate { get; set; }
        public AdministrativeDataViewModel AdministrativeData { get; set; }
        public List<ThesaurusEntryTranslationViewModel> Translations { get; set; }
        public List<ThesaurusEntryCodingSystemViewModel> CodingSystems { get; set; }
        public List<O4CodeableConceptDataOut> Codes { get; set; }
        public ThesaurusEntryTranslationViewModel GetTranslation(string language)
        {
            return this.Translations.FirstOrDefault(x => x.Language.Equals(language)) ?? new ThesaurusEntryTranslationViewModel() { Language = language};
        }

        public string GetAbbreaviations(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.Abbreviations);
        }

        public string GetSynonyms(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.Synonyms);
        }

        public string GetSimilarTerms(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.SimilarTerms);
        }

        public string GetDefinitionByTranslationOrDefault(string language)
        {
            string result = string.Empty;
            if (Translations.FirstOrDefault(x => x.Language.Equals(language)) != null)
            {
                result = Translations.FirstOrDefault(x => x.Language.Equals(language)).Definition;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = Translations.FirstOrDefault(x => x.Language.Equals("en")).Definition;
            }
            return result;
        }

        public string GetPreferredTermByTranslationOrDefault(string language)
        {
            string result = string.Empty;
            if (Translations.FirstOrDefault(x => x.Language.Equals(language)) != null)
            {
                result = Translations.FirstOrDefault(x => x.Language.Equals(language)).PreferredTerm;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = Translations.FirstOrDefault(x => x.Language.Equals("en")).PreferredTerm;
            }
            return result;
        }

        private string JoinList(List<string> list)
        {
            string result = "";
            if (list != null)
            {
                result = string.Join(",", list);
            }
            return result;
        }
    }
}