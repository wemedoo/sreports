using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldSetDataOut
    {
        [DataProp]
        public string FhirType { get; set; }
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Label { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        public List<FieldDataOut> Fields { get; set; } = new List<FieldDataOut>();
        [DataProp]
        public FormLayoutStyleDataOut LayoutStyle { get; set; }
        [DataProp]
        public bool IsBold { get; set; }
        [DataProp]
        public string MapAreaId { get; set; }
        [DataProp]
        public FormHelpDataOut Help { get; set; }
        [DataProp]
        public bool IsRepetitive { get; set; }
        [DataProp]
        public int NumberOfRepetitions { get; set; }
        public string InstanceId { get; set; }


    }
}