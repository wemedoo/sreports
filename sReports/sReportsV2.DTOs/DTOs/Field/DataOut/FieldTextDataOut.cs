using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldTextDataOut : FieldStringDataOut
    {
        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldText.cshtml";

        [JsonIgnore]
        public override string NestableView { get; } = "~/Views/Form/DragAndDrop/NestableFields/NestableTextField.cshtml";

        [DataProp]
        public int? MinLength { get; set; }
        [DataProp]
        public int? MaxLength { get; set; }

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