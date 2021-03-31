using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldTextDataOut : FieldStringDataOut
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldText.cshtml";

        [JsonIgnore]
        public override string ValidationAttr
        {
            get
            {
                string retVal = "";
                retVal += IsRequired ? " required " : "";
                retVal += MinLength != null ? " minlength=" + MinLength + " " : "";
                retVal += MaxLength != null ? " MaxLength=" + MaxLength + " " : "";

                return retVal;
            }
        }
    }
}