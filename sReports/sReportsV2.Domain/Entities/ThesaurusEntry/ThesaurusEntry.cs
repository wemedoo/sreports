using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    [BsonIgnoreExtraElements]
    public class ThesaurusEntry : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string O40MTId { get; set; }
        public string UmlsCode { get; set; }
        public string UmlsName { get; set; }
        public string UmlsDefinitions { get; set; }
        public List<ThesaurusEntryTranslation> Translations { get; set; }
        //public List<ThesaurusEntryCodingSystem> CodingSystems { get; set; }
       // public AdministrativeData AdministrativeData { get; set; }
        public List<O4CodeableConcept> Codes { get; set; }

        public ThesaurusState? State { get; set; }
        
        
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

        public string GetPreferredTermByTranslationOrDefault(string language, string activeLanguage=null)
        {
            string result = GetPreferredTermTranslation(language);
            
            if(string.IsNullOrWhiteSpace(result))
            {
                result = GetPreferredTermDefaultTranslation(activeLanguage);
            }

            return result;
        }

        public string GetPreferredTermByActiveLanguage(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm != null ? Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm : "";
        }

        public void SetPrefferedTermAndDescriptionForLang(string language, string preferredTerm, string definition)
        {

            ThesaurusEntryTranslation translation = this.Translations.FirstOrDefault(x => x.Language.Equals(language));
            if (translation == null)
            {
                translation = new ThesaurusEntryTranslation();
                translation.Language = language;
                Translations.Add(translation);
            }

            if (!string.IsNullOrWhiteSpace(definition))
            {
                translation.Definition = definition;
            }

            if (!string.IsNullOrWhiteSpace(preferredTerm))
            {
                translation.PreferredTerm = preferredTerm;
            }
        }

        private string GetPreferredTermDefaultTranslation(string activeLanguage=null)
        {
            string result = GetPreferredTermTranslation("en");
            if (string.IsNullOrWhiteSpace(result))
            {
                result = activeLanguage != null ? GetPreferredTermTranslation(activeLanguage) : result;
                result = string.IsNullOrWhiteSpace(result) ? GetPrefferedTermFirstTranslation() : result;
            }
            return result;
        }

        public string GetPreferredTermTranslation(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language))?.PreferredTerm;
        }

        private string GetPrefferedTermFirstTranslation()
        {
            string result = string.Empty;
            var translation = Translations.FirstOrDefault(x => !string.IsNullOrEmpty(x.PreferredTerm));
            if(translation != null)
            {
                result = translation.PreferredTerm;
            }

            return result;
        }

        public void MergeTranslations(ThesaurusEntry fromThesaurus) 
        {
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));

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
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));

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
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));

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
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));

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
        public void MergeUmlsCode(ThesaurusEntry fromThesaurus)
        {
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));
            this.UmlsCode = string.IsNullOrWhiteSpace(this.UmlsCode) ? fromThesaurus.UmlsCode : this.UmlsCode;
        }
        public void MergeUmlsName(ThesaurusEntry fromThesaurus)
        {
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));
            this.UmlsName = string.IsNullOrWhiteSpace(this.UmlsName) ? fromThesaurus.UmlsName : this.UmlsName;
        }

        public void MergeUmlsDefinitions(ThesaurusEntry fromThesaurus)
        {
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));
            this.UmlsDefinitions = string.IsNullOrWhiteSpace(this.UmlsDefinitions) ? fromThesaurus.UmlsDefinitions : this.UmlsDefinitions;
        }

        public void MergeCodes(ThesaurusEntry fromThesaurus)
        {
            Ensure.IsNotNull(fromThesaurus, nameof(fromThesaurus));

            if (fromThesaurus.Codes != null) 
            {
                foreach (var code in fromThesaurus.Codes)
                {
                    if (this.Codes.FirstOrDefault(x => x.System == code.System) == null)
                    {
                        this.Codes.Add(code);
                    }
                }
            }
        }
    }
}
