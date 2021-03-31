using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormFieldDependableDataOut
    {
        public string Condition { get; set; }
        public FormFieldDependableType? ActionType { get; set; }
        public string ActionParams { get; set; }
        public List<FormFieldDependableDataOut> Dependables { get; set; } = new List<FormFieldDependableDataOut>();
    }
}