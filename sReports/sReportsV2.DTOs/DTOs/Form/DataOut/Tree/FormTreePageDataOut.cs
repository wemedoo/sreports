using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut.Tree
{
    public class FormTreePageDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public List<FormTreeFieldSetDataOut> FieldSets { get; set; } = new List<FormTreeFieldSetDataOut>();
        public List<FormTreeFieldSetDataOut> SelectFieldSets(int thesaurusId)
        {
            List<FormTreeFieldSetDataOut> fieldSets = new List<FormTreeFieldSetDataOut>();
            List<FormTreeFieldDataOut> fields = new List<FormTreeFieldDataOut>();
            foreach (FormTreeFieldSetDataOut fieldSet in this.FieldSets)
            {
                if (fieldSet.ThesaurusId == thesaurusId)
                    fieldSets.Add(fieldSet);
                else
                {
                    fields = fieldSet.SelectFields(thesaurusId);

                    if (fields.Count > 0)
                        fieldSets.Add(fieldSet);
                }
            }
            return fieldSets;
        }
    }
}