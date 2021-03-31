using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldValueDataOut
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string ThesaurusId { get; set; }
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