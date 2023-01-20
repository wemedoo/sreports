using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
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
            string radioValue = this.Values.FirstOrDefault(x => x.ThesaurusId.ToString().Equals(referalValue))?.Label;
            if (radioValue == null)
            {
                if (referalValue.ShouldSetSpecialValue(IsRequired))
                {
                    return "N/E";
                }
            }
            return radioValue;
        }

        public override List<string> GetValueLabelsFromValue()
        {
            List<string> valueLabels = new List<string>();
            foreach (var val in Value)
            {
                string valueLabel = Values.FirstOrDefault(x => int.TryParse(val, out int parsedValue) && x.ThesaurusId == parsedValue)?.Label;
                if (!string.IsNullOrEmpty(valueLabel))
                {
                    valueLabels.Add(valueLabel);
                }
            }
            return valueLabels;
        }
    }
}
