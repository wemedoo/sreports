using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormFieldValueDataIn
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string ThesaurusId { get; set; }
        public double? NumericValue { get; set; }
    }
}