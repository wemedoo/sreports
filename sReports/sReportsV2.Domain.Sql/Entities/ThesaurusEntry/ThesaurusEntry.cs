using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusEntry : Entity
    {
        public int Id { get; set; }
        public ThesaurusState? State { get; set; }
        public List<ThesaurusEntryTranslation> Translations { get; set; }
        public AdministrativeData AdministrativeData { get; set; }
        public List<O4CodeableConcept> Codes { get; set; }
        public ThesaurusEntry() { }
        public ThesaurusEntry(string umlsCode) 
        {
            this.EntryDatetime = DateTime.Now;
            this.LastUpdate = DateTime.Now;
            this.State = ThesaurusState.Draft;
            this.Translations = new List<ThesaurusEntryTranslation>();
            this.Codes = new List<O4CodeableConcept>() 
            {
            };
        }

        public void SetTranslation(string definition, string language) 
        {
            ThesaurusEntryTranslation translation = this.Translations.FirstOrDefault(x => x.Language == language);
            
            if(translation == null)
            {
                this.Translations.Add(new ThesaurusEntryTranslation()
                {
                    Language = language,
                    Definition = definition
                });
            }
        }

        public void SetCode(O4CodeableConcept code) 
        {
            this.Codes.Add(code);
        }

        public string GetDefinitionByTranslationOrDefault(string language, string activeLanguage)
        {
            string result = string.Empty;
            ThesaurusEntryTranslation translation = Translations.FirstOrDefault(x => x.Language.Equals(language));
            if (translation != null)
            {
                result = translation.Definition;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = Translations.FirstOrDefault(x => x.Language.Equals("en"))?.Definition;
                if (string.IsNullOrEmpty(result))
                {
                    result = Translations.FirstOrDefault(x => x.Language.Equals(activeLanguage))?.Definition;
                }
            }
            return result;
        }


        public string GetPreferredTermByTranslationOrDefault(string language, string activeLanguage = null)
        {
            string result = GetPreferredTermTranslation(language);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = GetPreferredTermDefaultTranslation(activeLanguage);
            }

            return result;
        }

        public string GetPreferredTermTranslation(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm;
        }

        private string GetPreferredTermDefaultTranslation(string activeLanguage = null)
        {
            string result = GetPreferredTermTranslation("en");
            if (string.IsNullOrWhiteSpace(result))
            {
                result = activeLanguage != null ? GetPreferredTermTranslation(activeLanguage) : result;
                result = string.IsNullOrWhiteSpace(result) ? GetPrefferedTermFirstTranslation() : result;
            }
            return result;
        }

        private string GetPrefferedTermFirstTranslation()
        {
            string result = string.Empty;
            var translation = Translations.FirstOrDefault(x => !string.IsNullOrEmpty(x.PreferredTerm));
            if (translation != null)
            {
                result = translation.PreferredTerm;
            }

            return result;
        }

        public void MergeTranslations(ThesaurusEntry fromThesaurus)
        {

            this.AddMissingTranslations(fromThesaurus);

            foreach (var translation in this.Translations)
            {
                if (string.IsNullOrWhiteSpace(translation.PreferredTerm))
                {
                    ThesaurusEntryTranslation trans = fromThesaurus.Translations.FirstOrDefault(x => x.Language == translation.Language);
                    translation.PreferredTerm = trans?.PreferredTerm;
                    translation.Synonyms = trans?.Synonyms;
                    translation.Definition = trans?.Definition;
                    translation.Abbreviations = trans?.Abbreviations;
                }
            }
        }

        private void AddMissingTranslations(ThesaurusEntry fromThesaurus)
        {

            foreach (var tr in fromThesaurus.Translations)
            {
                if (this.Translations?.FirstOrDefault(x => x.Language == tr.Language) == null)
                {
                    this.Translations.Add(tr);
                }
            }
        }

        public void MergeSynonyms(ThesaurusEntry fromThesaurus)
        {

            foreach (ThesaurusEntryTranslation translation in this.Translations)
            {
                ThesaurusEntryTranslation fromThesaurusTranslation = fromThesaurus.Translations.FirstOrDefault(x => x.Language == translation.Language);

                if (fromThesaurusTranslation != null && fromThesaurusTranslation.Synonyms != null)
                {
                    foreach (string synonym in fromThesaurusTranslation.Synonyms)
                    {
                        if (translation.Synonyms != null && !translation.Synonyms.Contains(synonym, StringComparer.OrdinalIgnoreCase))
                        {
                            translation.Synonyms.Add(synonym);
                        }
                    }
                }
            }
        }

        public void MergeAbbreviations(ThesaurusEntry fromThesaurus)
        {

            foreach (ThesaurusEntryTranslation translation in this.Translations)
            {
                ThesaurusEntryTranslation fromThesaurusTranslation = fromThesaurus.Translations.FirstOrDefault(x => x.Language == translation.Language);
                if (fromThesaurusTranslation != null && fromThesaurusTranslation.Abbreviations != null)
                {
                    foreach (string abbreviation in fromThesaurusTranslation.Abbreviations.Where(x => translation.Abbreviations != null && !translation.Abbreviations.Contains(x, StringComparer.OrdinalIgnoreCase)))
                    {
                        translation.Abbreviations.Add(abbreviation);
                    }
                }


            }
        }
       
      

       

        public void MergeCodes(ThesaurusEntry fromThesaurus)
        {

            if (fromThesaurus.Codes != null)
            {
                foreach (var code in fromThesaurus.Codes)
                {
                    if (this.Codes.FirstOrDefault(x => x.System.Id == code.System.Id) == null)
                    {
                        this.Codes.Add(code);
                    }
                }
            }
        }

        public string GetPreferredTermByActiveLanguage(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm != null ? Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm : "";
        }


    }
}
