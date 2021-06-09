using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldSelectDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldSelect.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/FieldSelect.cshtml";
        public override string GetValue()
        {
            return this.Value?[0];
        }
    }
}