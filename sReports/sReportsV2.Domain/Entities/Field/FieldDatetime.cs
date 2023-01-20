using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Entities.FieldEntity
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(FieldTypes.Datetime)]
    public class FieldDatetime : FieldString
    {
        public override string Type { get; set; } = FieldTypes.Datetime;

        public override string GetReferrableValue(string referalValue)
        {
            if (referalValue != null)
            {
                if (referalValue.ShouldSetSpecialValue(IsRequired))
                {
                    referalValue = "N/E";
                }
                else
                {
                    referalValue = $"{referalValue.RenderDate()} {referalValue.RenderTime()}";
                }
            }
            return referalValue ?? string.Empty;
        }

        protected override string GetSingleValue(string value, string neTranslated)
        {
            return value.ShouldSetSpecialValue(IsRequired) ? neTranslated : GetReferrableValue(value);
        }
    }
}
