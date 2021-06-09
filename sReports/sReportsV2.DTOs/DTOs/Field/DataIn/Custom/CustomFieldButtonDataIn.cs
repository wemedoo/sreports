using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn.Custom
{
    public class CustomFieldButtonDataIn : FieldDataIn
    {
        public Action.CustomActionDataIn CustomAction { get; set; }
    }
}