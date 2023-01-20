using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldNumericDataOut : FieldStringDataOut
    {
        [DataProp]
        public double? Min { get; set; }

        [DataProp]
        public double? Max { get; set; }

        [DataProp]
        public double? Step { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldNumber.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableNumberField.cshtml";

        [JsonIgnore]
        public override string ValidationAttr
        {
            get
            {
                string retVal = "";
                retVal += IsRequired ? " required " : "";
                retVal += Min != null ? " Min=" + Min + " " : "";
                retVal += Max != null ? " Max=" + Max + " " : "";
                retVal += Step != null ? " Step=" + Step + " " : "";

                return retVal;
            }
        }
    }
}