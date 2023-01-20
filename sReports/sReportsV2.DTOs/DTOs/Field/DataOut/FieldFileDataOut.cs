using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldFileDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldFile.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableFileField.cshtml";

        public override string GetSynopticValue(string value, string neTranslated)
        {
            return value.ShouldSetSpecialValue(IsRequired) ? neTranslated : value.GetFileNameFromUri();
        }
    }
}