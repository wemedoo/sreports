using Hl7.Fhir.Model;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormChapter
    {
        public O4CodeableConcept Code { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsReadonly { get; set; } 
        public List<FormPage> Pages { get; set; } = new List<FormPage>();

        public int GetNumberOfFieldsForChapter()
        {
            return this.Pages
                .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields
                                    )
                                )
                 )
                .ToList()
                .Count;
        }

        public List<Field> GetAllFields()
        {
            return this.Pages
                        .SelectMany(page => page.ListOfFieldSets
                            .SelectMany(fieldSet => fieldSet
                                .SelectMany(fieldset => fieldset.Fields    
                                )
                            )
                        )
                        .ToList();
        }

        public List<Field> GetFieldsByList(List<string> fields)
        {
            List<Field> result = Pages.SelectMany(x => x.ListOfFieldSets.SelectMany(list => list.SelectMany(y => y.Fields.Where(f => fields.Contains(f.FhirType))))).ToList();
            foreach (Field field in result) 
            {
                field.Value = field.Value != null && field.Value.Count > 0 ? field.Value : null; 
            }

            return result;
        }

        public List<string> GetAllThesaurusIds()
        {
            List<string> thesaurusList = new List<string>();
            foreach (FormPage page in Pages)
            {
                var pageThesaurusId = page.ThesaurusId;
                thesaurusList.Add(pageThesaurusId);
                thesaurusList.AddRange(page.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormPage page in Pages)
            {
                page.Title = entries.FirstOrDefault(x => x.O40MTId.Equals(page.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                page.Description = entries.FirstOrDefault(x => x.O40MTId.Equals(page.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                page.GenerateTranslation(entries, language, activeLanguage);
            }  
        }

    }
}
