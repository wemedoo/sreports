using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Administration
{
    public class AdministrationFilterDataIn : DataIn
    {
        public CustomEnumType? PredefinedType { get; set; }
        public string PreferredTerm { get; set; }

    }
}