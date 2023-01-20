using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDatetimeDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldDatetime.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableDatetimeField.cshtml";

        public string RenderDateTime(string date, string time)
        {
            string dateTimeValue = "";
            if (!string.IsNullOrWhiteSpace(date))
            {
                if (!string.IsNullOrWhiteSpace(time))
                {
                    dateTimeValue = string.Concat(date, "T", time);
                }
                else
                {
                    dateTimeValue = string.Concat(date, "T12:00");
                }
            }
            return dateTimeValue;
        }

        public override string GetSynopticValue(string value, string neTranslated)
        {
            return value.ShouldSetSpecialValue(IsRequired) ? neTranslated : $"{value.RenderDate()} {value.RenderTime()}";
        }
    }
}