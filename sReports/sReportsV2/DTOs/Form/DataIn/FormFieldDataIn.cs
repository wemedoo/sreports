using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormFieldDataIn
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public List<string> Value { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }
        public string Unit { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public List<FormFieldValueDataIn> Values { get; set; } = new List<FormFieldValueDataIn>();
        public List<FormFieldDependableDataIn> Dependables { get; set; } = new List<FormFieldDependableDataIn>();
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }
        public bool IsBold { get; set; }
        public FormHelpDataIn Help { get; set; }
        public bool IsHiddenOnPdf { get; set; }
        public string Formula { get; set; }
        public string Regex { get; set; }
        public string RegexDescription { get; set; }
    }
}