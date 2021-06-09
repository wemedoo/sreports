using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class FormTreeFieldSetDataOut
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormTreeFieldDataOut> Fields { get; set; } = new List<FormTreeFieldDataOut>();
        public List<FormTreeFieldDataOut> SelectFields(int thesaurusId)
        {
            List<FormTreeFieldDataOut> fields = new List<FormTreeFieldDataOut>();
            foreach (FormTreeFieldDataOut fieldSet in this.Fields)
            {
                if (fieldSet.ThesaurusId == thesaurusId)
                    fields.Add(fieldSet);
                else
                {
                    List<FormTreeFieldValueDataOut> fieldValues = fieldSet.SelectFieldValues(thesaurusId);

                    if (fieldValues.Count > 0)
                        fields.Add(fieldSet);
                }

            }
            return fields;
        }
    }
}