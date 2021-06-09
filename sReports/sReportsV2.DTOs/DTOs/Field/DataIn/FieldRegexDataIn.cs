using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldRegexDataIn : FieldStringDataIn
    {
        public string Regex { get; set; }
        public string RegexDescription { get; set; }
    }
}