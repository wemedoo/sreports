using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldRegexDataOut : FieldStringDataOut
    { 
        public string Regex { get; set; }
        public string RegexDescription { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldRegex.cshtml";
    }
}