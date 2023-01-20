using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class ThesaurusEntry : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ThesaurusEntryId")]
        public int ThesaurusEntryId { get; set; }
        public ThesaurusState? State { get; set; }
        public List<ThesaurusEntryTranslation> Translations { get; set; }
        [ForeignKey("AdmnistrativeDataId")]
        public AdministrativeData AdministrativeData { get; set; }
        [Column("AdministrativeDataId")]
        public int? AdmnistrativeDataId { get; set; }  
        public List<O4CodeableConcept> Codes { get; set; } = new List<O4CodeableConcept>();
        // start Thesaurus Global specific properties
        public string PreferredLanguage { get; set; }
        public string UriClassLink { get; set; }
        public string UriClassGUI { get; set; }
        public string UriSourceLink { get; set; }
        public string UriSourceGUI { get; set; }
        // end Thesaurus Global specific properties
        public ThesaurusEntry() { }
        public ThesaurusEntry(string umlsCode) 
        {
            this.State = ThesaurusState.Draft;
            this.Translations = new List<ThesaurusEntryTranslation>();
        }

        public void SetTranslation(string definition, string language) 
        {
            ThesaurusEntryTranslation translation = GetTranslation(language);
            
            if(translation == null)
            {
                this.Translations.Add(new ThesaurusEntryTranslation()
                {
                    Language = language,
                    Definition = definition
                });
            }
        }
        public void UpdateTranslation(ThesaurusEntryTranslation thesaurusEntryTranslation)
        {
            ThesaurusEntryTranslation translationToUpdate = this.Translations.First(tr => tr.Language.Equals(thesaurusEntryTranslation.Language));
            translationToUpdate.Copy(thesaurusEntryTranslation);
        }

        public void SetCode(O4CodeableConcept code) 
        {
            this.Codes.Add(code);
        }

        public string GetDefinitionByTranslationOrDefault(string language, string activeLanguage)
        {
            string result = string.Empty;
            ThesaurusEntryTranslation translation = GetTranslation(language);
            if (translation != null)
            {
                result = translation.Definition;
            }

            if (string.IsNullOrEmpty(result))
            {
                result = GetTranslation("en")?.Definition;
                if (string.IsNullOrEmpty(result))
                {
                    result = GetTranslation(activeLanguage)?.Definition;
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

        public void MergeTranslations(ThesaurusEntry sourceThesaurus)
        {
            this.AddMissingTranslations(sourceThesaurus);

            foreach (var sourceTranslation in sourceThesaurus.Translations)
            {
                if (!string.IsNullOrWhiteSpace(sourceTranslation.PreferredTerm))
                {
                    ThesaurusEntryTranslation targetTranslation = GetTranslation(sourceTranslation.Language);
                    targetTranslation.PreferredTerm = sourceTranslation.PreferredTerm;
                    MergeAbbreviations(targetTranslation, sourceTranslation);
                    MergeDefinition(targetTranslation, sourceTranslation);
                    MergeSynonyms(targetTranslation, sourceTranslation);
                }
            }
        }

        public void MergeSynonyms(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeSynonyms(targetTranslation, sourceTranslation);
            }
        }

        public void MergeAbbreviations(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeAbbreviations(targetTranslation, sourceTranslation);
            }
        }

        public void MergeDefinitions(ThesaurusEntry sourceThesaurus)
        {
            foreach (ThesaurusEntryTranslation targetTranslation in this.Translations)
            {
                ThesaurusEntryTranslation sourceTranslation = sourceThesaurus.GetTranslation(targetTranslation.Language);
                MergeDefinition(targetTranslation, sourceTranslation);
            }
        }

        private void MergeDefinition(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && !string.IsNullOrEmpty(sourceTranslation.Definition))
            {
                targetTranslation.Definition = string.IsNullOrEmpty(targetTranslation.Definition) ? sourceTranslation.Definition : targetTranslation.Definition + " " + sourceTranslation.Definition;
            }
        }

        public void MergeCodes(ThesaurusEntry fromThesaurus)
        {
            if (fromThesaurus.Codes != null)
            {
                foreach (var code in fromThesaurus.Codes)
                {
                    if (this.Codes.FirstOrDefault(x => x.System.CodeSystemId == code.System.CodeSystemId) == null)
                    {
                        this.Codes.Add(code);
                    } 
                    else if (this.Codes.FirstOrDefault(x => x.System.CodeSystemId == code.System.CodeSystemId && x.Value == code.Value) == null)
                    {
                        this.Codes.Add(code);
                    }
                }
            }
        }

        public string GetPreferredTermByActiveLanguage(string language)
        {
            return GetTranslation(language)?.PreferredTerm ?? string.Empty;
        }

        public void Copy(ThesaurusEntry thesaurusEntry, bool copyCodes = true)
        {
            State = thesaurusEntry.State;
            PreferredLanguage = thesaurusEntry.PreferredLanguage;
            CopyTranslations(thesaurusEntry.Translations);
            CopyConnectionWithOntology(thesaurusEntry);
            if (copyCodes)
            {
                CopyCodes(thesaurusEntry.Codes);
            }
        }

        public void CopyConnectionWithOntology(ThesaurusEntry fromThesaurus)
        {
            this.UriClassLink = fromThesaurus.UriClassLink;
            this.UriClassGUI = fromThesaurus.UriClassGUI;
            this.UriSourceLink = fromThesaurus.UriSourceLink;
            this.UriSourceGUI = fromThesaurus.UriSourceGUI;
        }

        private string GetPreferredTermTranslation(string language)
        {
            return GetTranslation(language)?.PreferredTerm;
        }

        private ThesaurusEntryTranslation GetTranslation(string language)
        {
            return Translations.FirstOrDefault(x => x.Language.Equals(language));
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

        private void AddMissingTranslations(ThesaurusEntry sourceThesauus)
        {
            foreach (var tr in sourceThesauus.Translations)
            {
                if (this.Translations?.FirstOrDefault(x => x.Language == tr.Language) == null)
                {
                    this.Translations.Add(tr);
                }
            }
        }

        private void MergeSynonyms(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && sourceTranslation.Synonyms != null)
            {
                foreach (string sourceSynonym in sourceTranslation.Synonyms.Where(x => !targetTranslation.Synonyms.Contains(x, StringComparer.OrdinalIgnoreCase)))
                {
                    targetTranslation.Synonyms.Add(sourceSynonym);
                }
            }
        }

        private void MergeAbbreviations(ThesaurusEntryTranslation targetTranslation, ThesaurusEntryTranslation sourceTranslation)
        {
            if (sourceTranslation != null && sourceTranslation.Abbreviations != null)
            {
                foreach (string sourceAbbreviation in sourceTranslation.Abbreviations.Where(x => !targetTranslation.Abbreviations.Contains(x, StringComparer.OrdinalIgnoreCase)))
                {
                    targetTranslation.Abbreviations.Add(sourceAbbreviation);
                }
            }
        }

        private void CopyTranslations(List<ThesaurusEntryTranslation> translations)
        {
            foreach (var translation in this.Translations)
            {
                var t = translations.FirstOrDefault(x => x.Language == translation.Language);
                if (t != null)
                {
                    translation.PreferredTerm = t.PreferredTerm;
                    translation.Definition = t.Definition;
                    translation.Synonyms = t.Synonyms;
                    translation.Abbreviations = t.Abbreviations;
                }
            }

            foreach (var t in translations.Where(x => !Translations.Select(c => c.Language).Contains(x.Language)).ToList())
            {
                Translations.Add(t);
            }
        }

        private void CopyCodes(List<O4CodeableConcept> codes)
        {
            DeleteExistingRemovedCodes(codes);
            AddNewOrUpdateOldCodes(codes);
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

        private void DeleteExistingRemovedCodes(List<O4CodeableConcept> upcomingCodes)
        {
            List<O4CodeableConcept> remainingCodes = new List<O4CodeableConcept>();
            foreach (var code in Codes)
            {
                var remainingCode = upcomingCodes.Any(x => x.O4CodeableConceptId == code.O4CodeableConceptId);
                if (remainingCode)
                {
                    remainingCodes.Add(code);
                }
            }
            Codes = remainingCodes;
        }

        private void AddNewOrUpdateOldCodes(List<O4CodeableConcept> upcomingCodes)
        {
            foreach (var code in upcomingCodes)
            {
                if (code.O4CodeableConceptId == 0)
                {
                    Codes.Add(code);
                }
                else
                {
                    var dbCode = Codes.FirstOrDefault(x => x.O4CodeableConceptId == code.O4CodeableConceptId);
                    dbCode.Copy(code);
                }
            }
        }
    }
}
