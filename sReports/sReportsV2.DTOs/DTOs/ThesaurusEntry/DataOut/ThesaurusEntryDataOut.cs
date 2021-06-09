using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ThesaurusEntry.DataOut
{
    public class ThesaurusEntryDataOut
    {
        public int Id { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public string UmlsDefinitions { get; set; }
        public DateTime? LastUpdate { get; set; }
        public AdministrativeDataDataOut AdministrativeData { get; set; }
        public List<ThesaurusEntryTranslationDTO> Translations { get; set; }
        public List<ThesaurusEntryCodingSystemDTO> CodingSystems { get; set; }
        public List<O4CodeableConceptDataOut> Codes { get; set; }
        public ThesaurusState State { get; set; }
        public ThesaurusEntryTranslationDTO GetTranslation(string language)
        {
            return this.Translations.FirstOrDefault(x => x.Language.Equals(language)) ?? new ThesaurusEntryTranslationDTO() { Language = language };
        }

        public string GetAbbreaviations(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.Abbreviations);
        }

        public string GetSynonyms(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.Synonyms);
        }

        public List<string> GetSynonymsListByLanguage(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.Synonyms;
        }

        public List<string> GetAbbreviationListByLanguage(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.Abbreviations;
        }

        public List<SimilarTermDTO> GetSimilarTermsListByLanguage(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.SimilarTerms;
        }

        public string GetSimilarTerms(string language)
        {
            return JoinList(Translations.FirstOrDefault(x => x.Language.Equals(language))?.SimilarTerms.Select(x => x.Name).ToList());
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
                if (Translations.FirstOrDefault(x => x.Language.Equals("en")) != null)
                    result = Translations.FirstOrDefault(x => x.Language.Equals("en")).Definition;
                else
                    result = Translations.FirstOrDefault(x => x.Language != null).Definition;
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
                if (Translations.FirstOrDefault(x => x.Language.Equals("en")) != null)
                    result = Translations.FirstOrDefault(x => x.Language.Equals("en")).PreferredTerm;
                else
                    result = Translations.FirstOrDefault(x => x.Language != null).PreferredTerm;
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