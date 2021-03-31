using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldCalculativeDataOut : FieldStringDataOut
    {
        public string Formula { get; set; }

        public string GetFormulaNormilized()
        {
            return this.Formula
                    .Replace("[", "")
                    .Replace("]", "")
                    .Trim();
        }

        public List<string> GetFormulaFields()
        {
            List<string> result = new List<string>();

            string[] spliited = Formula.Split('[');
            foreach (string split in spliited.Where(x => x.Contains("]")))
            {
                string fieldData = split.Trim();
                int indexOfBracket = fieldData.IndexOf("]");
                string fieldId = fieldData.Substring(0, indexOfBracket);
                result.Add(fieldId);
            }
            return result;
        }

        [JsonIgnore]
        public override string PartialView { get; } = "~/Views/Form/FieldCalculative.cshtml";

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