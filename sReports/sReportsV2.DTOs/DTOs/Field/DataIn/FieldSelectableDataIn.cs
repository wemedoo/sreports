using sReportsV2.DTOs.Form.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldSelectableDataIn : FieldDataIn
    {
        public List<FormFieldValueDataIn> Values { get; set; } = new List<FormFieldValueDataIn>();
        public List<FormFieldDependableDataIn> Dependables { get; set; } = new List<FormFieldDependableDataIn>();
    }
}