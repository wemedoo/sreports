using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.ThesaurusEntry
{
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
        public AdministrativeData AdministrativeData { get; set; }
        public List<O4CodeableConcept> Codes { get; set; }
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

        private string GetPreferredTermTranslation(string language)
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
    }
}
