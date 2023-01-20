using sReportsV2.Common.Extensions;
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
                        else if (field.Value.Contains(copyFromValue.ThesaurusId.ToString()))
                        {
                            result.Add(value.ThesaurusId.ToString());
                        }
                    }
                }
            }

            this.Value = new List<string>() { string.Join(",", result) };
        }

        public override List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (FormFieldValue value in Values)
            {
                var fieldValuehesaurusId = value.ThesaurusId;
                thesaurusList.Add(fieldValuehesaurusId);
            }

            return thesaurusList;
        }

        public override void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormFieldValue value in Values)
            {
                value.Label = entries.FirstOrDefault(x => x.ThesaurusEntryId.Equals(value.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
            }
        }

        public override void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;

            foreach (FormFieldValue value in this.Values)
            {
                value.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

        public override string GetPatholinkValue(string neTranslated, string optionValue = "")
        {
            string patholinkValue = string.Empty;
            if (Value != null && Value.Count > 0)
            {
                patholinkValue = Value[0];
                if (patholinkValue.ShouldSetSpecialValue(IsRequired))
                {
                    patholinkValue = neTranslated;
                }
                else
                {
                    patholinkValue = IsMatchValue(patholinkValue, optionValue) ? "true" : string.Empty;
                }
            }
            return patholinkValue;
        }

        private bool IsMatchValue(string fieldValue, string optionValue)
        {
            return fieldValue != null && fieldValue.Contains(optionValue);
        }
    }
}
