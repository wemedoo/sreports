using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldTextDataIn : FieldStringDataIn
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
}