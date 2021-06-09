using Hl7.Fhir.Model;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using MongoDB.Bson;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FieldSet
    {
        public O4CodeableConcept Code { get; set; } 
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set;}
        public List<Field> Fields { get; set; } = new List<Field>();
        public LayoutStyle LayoutStyle { get; set; }
        public Help Help { get; set; }
        public string MapAreaId { get; set; }
        public bool IsBold { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitionsForPdf { get; set; }
        public string InstanceId { get; set; }



        public Patient ParseFieldSetIntoPatient()
        {
            Patient patient = new Patient();
            if (!string.IsNullOrEmpty(this.FhirType) && this.FhirType.Equals("Patient"))
            {
                foreach (Field formField in this.Fields)
                {
                    switch (formField.FhirType)
                    {
                        case ResourceTypes.PatientName:
                             ParseFieldIntoName(formField, patient);
                            break;
                        case ResourceTypes.PatientGender:
                            ParseFieldIntoGender(formField, patient);
                            break;
                        case ResourceTypes.PatientIdentifier:
                            ParseFieldIntoIdentifier(formField, patient);
                            break;
                        default:
                            break;
                    }
                }

            }
            return patient;
        }
        private void ParseFieldIntoName(Field formField, Patient patient)
        {
            if (!string.IsNullOrEmpty(formField.Value?[0]))
            {
                HumanName humanName = new HumanName();
                humanName.GivenElement.Add(new FhirString(formField.Value?[0].Split(' ')[0]));
                try
                {
                    if (!string.IsNullOrEmpty(formField.Value?[0].Split(' ')[1]))
                    {
                        humanName.FamilyElement = new FhirString(formField.Value?[0].Split(' ')[1]);

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                patient.Name.Add(humanName);
            }
        }
        private void ParseFieldIntoGender(Field formField, Patient patient)
        {
            if (formField.Value?[0] == "Male")
            {
                patient.Gender = AdministrativeGender.Male;
            }
            else if (formField.Value?[0] == "Female")
            {
                patient.Gender = AdministrativeGender.Female;
            }
            else if (formField.Value?[0] == "Other")
            {
                patient.Gender = AdministrativeGender.Other;
            }
            else
            {
                patient.Gender = AdministrativeGender.Unknown;
            }
        }
        private void ParseFieldIntoIdentifier(Field formField, Patient patient)
        {
            Identifier id = new Identifier(formField.Label.Split('-')[0], formField.Label.Split('-')[1]);
            patient.Identifier.Add(id);
        }

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (Field field in Fields)
            {
                var fieldhesaurusId = field.ThesaurusId;
                thesaurusList.Add(fieldhesaurusId);
                thesaurusList.AddRange(field.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (Field field in Fields)
            {
                field.Label = entries.FirstOrDefault(x => x.Id.Equals(field.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                field.Description = entries.FirstOrDefault(x => x.Id.Equals(field.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                field.GenerateTranslation(entries, language, activeLanguage);
            }
        }

        public bool IsReferable(FieldSet targetFieldSet) 
        {
            Ensure.IsNotNull(targetFieldSet, nameof(FieldSet));

            bool result = false;
            int matchedFieldCounter = 0;
            if (this.ThesaurusId == targetFieldSet.ThesaurusId && this.Fields.Count == targetFieldSet.Fields.Count) 
            {
                foreach (Field field in this.Fields) 
                {
                    foreach (Field targetField in this.Fields)
                    {
                        if (field.ThesaurusId == targetField.ThesaurusId && targetField.Type == field.Type) 
                        {
                            matchedFieldCounter++;
                            break;
                        }
                    }
                }

                if (matchedFieldCounter == this.Fields.Count) 
                {
                    result = true;
                }
            }

            return result;
        }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (Field field in this.Fields)
            {
                field.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }
    }
}
