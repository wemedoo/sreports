using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldStringDataOut : FieldDataOut
    {
        public bool IsRepetitive { get; set; }
        public List<string> RepetitiveValue { get; set; }
        public int NumberOfRepetitions { get; set; }

        public override string GetValue()
        {
            string result = string.Empty;
            foreach (string value in this.Value) 
            {
                result += value + Environment.NewLine;
            }
            return result;
        }

    }
}