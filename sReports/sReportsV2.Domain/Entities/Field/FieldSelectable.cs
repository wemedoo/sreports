using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    public class FieldSelectable : Field
    {
        public List<FormFieldValue> Values { get; set; } = new List<FormFieldValue>();
        public List<FormFieldDependable> Dependables { get; set; } = new List<FormFieldDependable>();

        public override void CopyValue(Field field)
        {
            FieldSelectable selectableField = (FieldSelectable)field;
            field = Ensure.IsNotNull(field, nameof(field));
            List<string> result = new List<string>();
            if(selectableField.Values != null)
            {
                foreach (FormFieldValue value in Values)
                {
                    FormFieldValue copyFromValue = selectableField.Values.FirstOrDefault(x => x.ThesaurusId.Equals(value.ThesaurusId));
                    if (field?.Value != null)
                    {
                        if (field.Value.Contains(copyFromValue.Value))
                        {
                            result.Add(value.Value);
                        }
                        else if (field.Value.Contains(copyFromValue.ThesaurusId))
                        {
                            result.Add(value.ThesaurusId);
                        }
                    }
                }
            }

            this.Value = new List<string>() { string.Join(",", result) };
        }

        public override List<string> GetAllThesaurusIds()
        {
            List<string> thesaurusList = new List<string>();
            foreach (FormFieldValue value in Values)
            {
                var fieldValuehesaurusId = value.ThesaurusId;
                thesaurusList.Add(fieldValuehesaurusId);
            }

            return thesaurusList;
        }

        public override void GenerateTranslation(List<ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormFieldValue value in Values)
            {
                value.Label = entries.FirstOrDefault(x => x.O40MTId.Equals(value.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
            }
        }
    }
}
