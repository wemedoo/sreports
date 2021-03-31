using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{ 
    public class FieldNumericDataIn : FieldStringDataIn
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }
    }
}