using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldStringDataIn : FieldDataIn
    {
        public bool IsRepetitive { get; set; }
        public List<string> RepetitiveValue { get; set; }
        public int NumberOfRepetitions { get; set; }


    }
}