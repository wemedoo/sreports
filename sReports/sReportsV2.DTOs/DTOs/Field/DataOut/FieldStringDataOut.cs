using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldStringDataOut : FieldDataOut
    {
        [DataProp]
        public bool IsRepetitive { get; set; }
        public List<string> RepetitiveValue { get; set; }
        [DataProp]
        public int NumberOfRepetitions { get; set; }

    }
}