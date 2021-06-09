using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class FormTreeFieldDataOut
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormTreeFieldValueDataOut> Values { get; set; } = new List<FormTreeFieldValueDataOut>();
        public List<FormTreeFieldValueDataOut> SelectFieldValues(int thesaurusId)
        {
            List<FormTreeFieldValueDataOut> fieldValues = new List<FormTreeFieldValueDataOut>();
            foreach (FormTreeFieldValueDataOut fieldValue in this.Values)
            {
                if (fieldValue.ThesaurusId == thesaurusId)
                    fieldValues.Add(fieldValue);

            }
            return fieldValues;
        }
    }
}