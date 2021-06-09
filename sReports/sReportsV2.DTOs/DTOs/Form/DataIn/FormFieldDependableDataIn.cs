using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormFieldDependableDataIn
    {
        public string Condition { get; set; }
        public FormFieldDependableType? ActionType { get; set; }
        public string ActionParams { get; set; }
    }
}