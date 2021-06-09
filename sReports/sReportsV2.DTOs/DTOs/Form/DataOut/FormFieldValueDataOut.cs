using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldValueDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Label { get; set; }
        [DataProp]
        public string Value { get; set; }
        [DataProp]
        public string ThesaurusId { get; set; }
        [DataProp]
        public double? NumericValue { get; set; }

        public string GetShortValue() {

            string shortValue = this.Label;
            
            if (this.Label.Length > 100) {
                shortValue = this.Label.Substring(0,100);
                shortValue += "... ";
            }

            return shortValue;
        }
    }
}