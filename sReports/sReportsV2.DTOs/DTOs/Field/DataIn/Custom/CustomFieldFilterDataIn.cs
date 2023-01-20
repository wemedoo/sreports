using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Field.DataIn.Custom
{
    public class CustomFieldFilterDataIn
    {
        public string FieldType { get; set; }
        public int FieldThesaurusId { get; set; }
        public string FieldLabel { get; set; }
        public string Value { get; set; }
        public string FilterOperator { get; set; }
    }
}
