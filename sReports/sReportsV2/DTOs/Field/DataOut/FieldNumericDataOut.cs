using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldNumericDataOut : FieldStringDataOut
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public double? Step { get; set; }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldNumber.cshtml";

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