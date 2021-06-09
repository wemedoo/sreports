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
    [BsonDiscriminator(FieldTypes.Radio)]
    public class FieldRadio : FieldSelectable
    {
        public override string Type { get; set; } = FieldTypes.Radio;
        
        public override string GetSelectedValue()
        {
            var selectedValue = this.Values.FirstOrDefault(x => x.ThesaurusId.ToString().Equals(this.Value));
            return selectedValue != null ? selectedValue.ThesaurusId.ToString() : string.Empty;
        }

        public override string GetReferrableValue(string referalValue)
        {
            return this.Values.FirstOrDefault(x => x.ThesaurusId.Equals(referalValue)).Label;
        }
    }
}
