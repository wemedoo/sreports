using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity.Custom;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(
        typeof(FieldText),
        typeof(FieldRadio),
        typeof(FieldCheckbox),
        typeof(FieldEmail),
        typeof(FieldDate),
        typeof(FieldCalculative),
        typeof(FieldRegex),
        typeof(FieldNumeric),
        typeof(FieldFile),
        typeof(FieldSelect),
        typeof(FieldTextArea),
        typeof(FieldDatetime),
        typeof(CustomFieldButton))]
    public class Field
    {
        public string InstanceId { get; set; }
        public string Performer { get; set; }
        public string FhirType { get; set; }
        public string Id { get; set; }
        public List<string> Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsRequired { get; set; }
        public bool IsBold { get; set; }
        public bool IsHiddenOnPdf { get; set; }
        public Help Help { get; set; }
        public O4CodeableConcept Code { get; set; }
        public O4ResourceReference Subject { get; set; }
        public List<O4ResourceReference> Result { get; set; }

        [BsonIgnore]
        public virtual string Type { get; set; }

        #region virtual methods
        public void SetValue(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this.Value = this.Value ?? new List<string>();
                this.Value.Add(value);
            }
        }

        public virtual void CopyValue(Field field)
        {
            field = Ensure.IsNotNull(field, nameof(field));
            this.Value = field.Value;
        }

        public virtual List<int> GetAllThesaurusIds()
        {
            return new List<int>();
        }

        public virtual void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {            
        }

        public virtual string GetSelectedValue()
        {
            return string.Empty;
        }

        public virtual string GetFieldValue()
        {
            return this.Value != null  && this.Value.Count > 0 ? this.Value[0] ?? string.Empty : string.Empty;
        }

        public virtual string GetReferrableValue(string referalValue)
        {
            return referalValue;
        }

        public string GetValue()
        {
            return this.Value != null ? string.Join(",", this.Value) : string.Empty;
        }

        public virtual void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
        }

        public virtual bool HasValue()
        {
            return Value != null
             && Value.Count > 0
             && !string.IsNullOrWhiteSpace(Value?[0].Replace(",", " "));
        }
        #endregion
    }
}
