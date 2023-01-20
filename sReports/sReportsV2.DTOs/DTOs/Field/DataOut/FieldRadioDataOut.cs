using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldRadioDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldRadio.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableRadioField.cshtml";
        public override string GetValue() 
        {
            return Values.FirstOrDefault(x => HasValue() && x.ThesaurusId.ToString() == Value[0])?.Label;
        }

        public override string GetSelectedValue()
        {
            return Value?.FirstOrDefault() ?? string.Empty;
        }

        public override string GetSynopticValue(string value, string neTranslated)
        {
            return value.ShouldSetSpecialValue(IsRequired) ? neTranslated : GetValue();
        }
    }
}