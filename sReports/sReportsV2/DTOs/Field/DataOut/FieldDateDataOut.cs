using Newtonsoft.Json;
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
        public override string ValidationAttr
        {
            get
            {
                string retVal = "";
                retVal += IsRequired ? " required " : "";
                return retVal;
            }
        }
    }
}