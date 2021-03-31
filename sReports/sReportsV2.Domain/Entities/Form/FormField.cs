using Hl7.Fhir.Model;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormField
    {
        public O4CodeableConcept Code { get; set; }
        public O4ResourceReference Subject { get; set; }
        public List<O4ResourceReference> Result { get; set; }
        public string Performer { get; set; }
        public string FhirType { get; set; }
        public string Id { get; set; }
        //public string Value { get; set; }
        public List<string> Value { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public List<FormFieldValue> Values { get; set; } = new List<FormFieldValue>();
        public List<FormFieldDependable> Dependables { get; set; } = new List<FormFieldDependable>();
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }
        public Help Help { get; set; }
        public bool IsBold { get; set; }
        public bool IsHiddenOnPdf { get; set; }
        public string Regex { get; set; }
        public string RegexDescription { get; set; }

        public void SetValue(string value) 
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.Value = this.Value ?? new List<string>();
                this.Value.Add(value);
            }
        }

        public string Formula { get; set; }
        public void CopyValue(FormField field)
        {
            field = Ensure.IsNotNull(field, nameof(field));

            if (this.Values != null && this.Values.Any())
            {
                CopyValueFromFormFieldValues(field);
            }
            else
            {
                this.Value = field.Value;
            }
        }


        public void CopyValueFromFormFieldValues(FormField field)
        {
            field = Ensure.IsNotNull(field, nameof(field));

            List<string> result = new List<string>();
            foreach (FormFieldValue value in Values)
            {
                FormFieldValue copyFromValue = field.Values.FirstOrDefault(x => x.ThesaurusId.Equals(value.ThesaurusId));
                if(field?.Value != null)
                {
                    if (field.Value.Contains(copyFromValue.Value))
                    {
                        result.Add(value.Value);
                    } 
                    else if (field.Value.Contains(copyFromValue.ThesaurusId))
                    {
                        result.Add(value.ThesaurusId);
                    }
                }
            }
            this.Value.Add(string.Join(",", result));
        }

        public bool IsCheckedField(FormFieldValue fieldValue)
        {
            fieldValue = Ensure.IsNotNull(fieldValue, nameof(fieldValue));

            bool result = false;
            List<string> checkedValues = new List<string>();

            if (this.Value != null)
            {
                checkedValues = this.Value[0].Split(',').ToList();
            }

            foreach (string valueChecked in checkedValues)
            {
                if (valueChecked == fieldValue.Value)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public List<string> GetAllThesaurusIds()
        {
            List<string> thesaurusList = new List<string>();
            foreach (FormFieldValue value in Values)
            {
                var fieldValuehesaurusId = value.ThesaurusId;
                thesaurusList.Add(fieldValuehesaurusId);
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormFieldValue value in Values)
            {
                value.Label = entries.FirstOrDefault(x => x.O40MTId.Equals(value.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
            }
        }

        public string GetSelectedValue()
        {
            var selectedValue = this.Values.FirstOrDefault(x => x.Value.Equals(this.Value));
            string selectValue = selectedValue != null ? selectedValue.Value : "";

            return selectValue;
        }

        public string GetReferrableValue(string referalValue) 
        {
            return this.Type == FieldTypes.Radio ? this.Values.FirstOrDefault(x => x.ThesaurusId.Equals(referalValue)).Label : referalValue;
        }

        public string GetReferrableRepetitiveValue()
        {
            string result = string.Empty;

            foreach (string repetititveValue in this.Value)
            {
                result += $"{repetititveValue} <br>";
            }

            return result;
        }
    }
}
