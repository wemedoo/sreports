using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldRegexDataOut : FieldStringDataOut
    {
        [DataProp]
        public string Regex { get; set; }

        [DataProp]
        public string RegexDescription { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldRegex.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableRegexField.cshtml";
    }
}