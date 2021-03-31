using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCheckboxDataOut : FieldSelectableDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldCheckbox.cshtml";


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
        public override string GetValue()
        {
            return this.Value?[0];
        }
    }
}