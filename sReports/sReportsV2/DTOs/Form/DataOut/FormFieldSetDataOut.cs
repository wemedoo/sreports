using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldSetDataOut
    {
        public string FhirType { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ThesaurusId { get; set; }
        public List<FieldDataOut> Fields { get; set; } = new List<FieldDataOut>();
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
        public bool IsBold { get; set; }
        public string MapAreaId { get; set; }
        public FormHelpDataOut Help { get; set; }
        public bool IsRepetitive { get; set; }
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }


    }
}