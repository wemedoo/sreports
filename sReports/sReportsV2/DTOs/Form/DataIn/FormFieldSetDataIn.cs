using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormFieldSetDataIn
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public List<FieldDataIn> Fields { get; set; } = new List<FieldDataIn>();
        public FormLayoutStyleDataIn LayoutStyle { get; set; }
        public bool IsBold { get; set; }
        public FormHelpDataIn Help { get; set; }
        public string MapAreaId { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }



    }
}