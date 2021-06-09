using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Field.DataOut.Custom.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut.Custom
{
    public class CustomFieldButtonDataOut : FieldDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/Custom/FieldCustomButton.cshtml";
        public override string NestableView { get; } = "~/Views/Form/Custom/FieldCustomButton.cshtml";


        [DataProp]
        public CustomActionDataOut CustomAction { get; set; }
    }
}