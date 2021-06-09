using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Select)]
    public class FieldSelect : FieldSelectable
    {
        public override string Type { get; set; } = FieldTypes.Select;
        public override string GetSelectedValue()
        {
            var selectedValue = this.Values.FirstOrDefault(x => x.Value.Equals(this.Value));
            string selectValue = selectedValue != null ? selectedValue.Value : "";

            return selectValue;
        }

        public override string GetFieldValue()
        {
            var selectedValue = this.Values.FirstOrDefault(x => x.Value.Equals(this.Value));
            string selectValue = selectedValue != null ? selectedValue.Value : "";

            return selectValue;
        }

        public override void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (FormFieldValue value in this.Values)
            {
                value.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }
    }
}
