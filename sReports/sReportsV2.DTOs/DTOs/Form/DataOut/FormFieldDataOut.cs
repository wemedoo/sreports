using Newtonsoft.Json;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldDataOut
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public List<string> Value { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }


        public string Description { get; set; }
        public string Unit { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public List<FormFieldValueDataOut> Values { get; set; } = new List<FormFieldValueDataOut>();
        public List<FormFieldDependableDataOut> Dependables { get; set; } = new List<FormFieldDependableDataOut>();
        public bool IsRequired { get; set; } = false;
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }
        public bool IsBold { get; set; }
        public string Formula {get;set;}
        public FormHelpDataOut Help { get; set; }
        public bool IsHiddenOnPdf { get; set; }

        public string Regex { get; set; }
        public string RegexDescription { get; set; }
        public void GetDependablesData(List<FormFieldDataOut> fields, List<FormFieldDependableDataOut> dependables)
        {
            dependables = Ensure.IsNotNull(dependables, nameof(dependables));

            foreach (FormFieldDependableDataOut formFieldDependable in dependables)
            {
                var dependableField = fields.FirstOrDefault(x => x.Id.Equals(formFieldDependable.ActionParams));
                if (dependableField != null)
                {
                    if (dependableField.Dependables != null && dependableField.Dependables.Any())
                    {
                        GetDependablesData(fields, dependableField.Dependables);
                        formFieldDependable.Dependables.AddRange(dependableField.Dependables);
                    }
                }
            }
        }

        public string GetFormulaNormilized()
        {
            return this.Formula
                    .Replace("[", "")
                    .Replace("]", "")
                    .Trim();
        }

        public List<string> GetFormulaFields()
        {
            List<string> result = new List<string>();

            string[] spliited = Formula.Split('[');
            foreach (string split in spliited.Where(x => x.Contains("]")))
            {
                string fieldData = split.Trim();
                int indexOfBracket = fieldData.IndexOf("]");
                string fieldId = fieldData.Substring(0, indexOfBracket);
                result.Add(fieldId);
            }
            return result;
        }

        public string GetFormFieldValue() 
        {
            string value = "";
            foreach (FormFieldValueDataOut val in Values)
            {
                if (val.ThesaurusId == Value?[0])
                {
                    value = val.Value;
                    break;
                }
                else
                {
                    value = Value?[0];
                }
            }
            return value;
        }
        /*
        public bool IsEmail { get; set; }
        public bool IsURL { get; set; }
        public bool IsDate { get; set; }
        public bool IsNumber { get; set; }
        public bool IsDigits { get; set; }
        */
        #region HTML Helper Attributes
        /// <summary> Label + * if required </summary>
        [JsonIgnore]
        public string FullLabel
        {
            get
            {
                string retVal = IsBold ? $"<b>{Label}</b>" : Label;
                if (!string.IsNullOrEmpty(Unit))
                    retVal += " (" + Unit + ")";
                if (IsRequired)
                    retVal += " * ";
                if (!string.IsNullOrEmpty(ThesaurusId))
                    retVal += " <a target='_blank' href='/ThesaurusEntry/EditByO4MtId?id=" + ThesaurusId + "' title='Thesaurus ID: " + ThesaurusId + "' class='metat-link' ><i class='far fa-question-circle'></i></a> ";
                return retVal;
                //https://uts.nlm.nih.gov/metathesaurus.html?cui=C0238463
                //http://vocabularies.unesco.org/thesaurus/
            }
        }

        [JsonIgnore]
        public string DescriptionLabel
        {
            get
            {
                return string.IsNullOrEmpty(Description) ? "Enter: " + Label : Description;
            }
        }

        [JsonIgnore]
        public string ValidationAttr
        {
            get
            {
                string retVal = "";
                retVal += IsRequired ? " required " : "";
                retVal += MinLength != null ? " minlength=" + MinLength + " " : "";
                retVal += MaxLength != null ? " MaxLength=" + MaxLength + " " : "";
                retVal += Min != null ? " Min=" + Min + " " : "";
                retVal += Max != null ? " Max=" + Max + " " : "";
                retVal += Step != null ? " Step=" + Step + " " : "";

                return retVal;
            }
        }

        [JsonIgnore]
        public string Visibility
        {
            get
            {
                string retVal = "";
                if (!IsVisible)
                    retVal = " style='display: none; ' ";
                return retVal;
            }
        }
    }
    #endregion HTML Helper Attributes
}