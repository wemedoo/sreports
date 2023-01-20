using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDateDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldDate.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableDateField.cshtml";
    }
}