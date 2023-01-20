using Newtonsoft.Json;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.Form.DataOut;
using System.Collections.Generic;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using System;
using System.Linq;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldDataOut
    {
        public virtual string NestableView { get; }
        public virtual string PartialView { get;}
        public string InstanceId { get; set; }
        [DataProp]
        public string FhirType { get; set; }
        [DataProp]
        public string Id { get; set; }
        public List<string> Value { get; set; }
        [DataProp]
        public string Type { get; set; }
        [DataProp]
        public string Label { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public string Unit { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public bool IsVisible { get; set; } = true;
        [DataProp]
        public bool IsReadonly { get; set; }
        [DataProp]
        public bool IsRequired { get; set; } = false;
        [DataProp]
        public bool IsBold { get; set; }
        [DataProp]
        public FormHelpDataOut Help { get; set; }
        [DataProp]
        public bool IsHiddenOnPdf { get; set; }
        public bool IsDisabled { get; set; }
        public virtual string GetValue() 
        {
            return this.Value?[0];
        }
        #region HTML Helper Attributes
        /// <summary> Label + * if required </summary>
        [JsonIgnore]
        public string FullLabel
        {
            get
            {
                string retVal = IsBold ? $"<b>{Label}</b>" : Label;
                if (!string.IsNullOrEmpty(Unit))
                    retVal += " (" + Unit + ")";
                if (IsRequired)
                    retVal += " * ";
                //if (!string.IsNullOrEmpty(ThesaurusId))
                //    retVal += " <a target='_blank' href='/ThesaurusEntry/EditByO4MtId?id=" + ThesaurusId + "' title='Thesaurus ID: " + ThesaurusId + "' class='metat-link' ><i class='far fa-question-circle'></i></a> ";
                return retVal;
                //https://uts.nlm.nih.gov/metathesaurus.html?cui=C0238463
                //http://vocabularies.unesco.org/thesaurus/
            }
        }

        [JsonIgnore]
        public virtual string DescriptionLabel
        {
            get
            {
                return string.IsNullOrEmpty(Description) ? "Enter: " + Label : Description;
            }
        }

        [JsonIgnore]
        public virtual string ValidationAttr
        {
            get
            {
                return IsRequired ? " required " : "";
            }
        }

        [JsonIgnore]
        public string Visibility
        {
            get
            {
                string retVal = "";
                if (!IsVisible)
                    retVal = " style='display: none; ' ";
                return retVal;
            }
        }

        [JsonIgnore]
        public string IsRequiredDataAttr
        {
            get
            {
                return string.Format("data-is-required=\"{0}\"", IsRequired);
            }
        }

        public bool HasValue()
        {
            return Value != null && Value.Count > 0;
        }

        public bool AcceptsSpecialValue
        {
            get
            {
                return Type != FieldTypes.CustomButton;
            }
        }

        public virtual string GetSynopticValue(string value, string neTranslated)
        {
            return value.ShouldSetSpecialValue(IsRequired) ? neTranslated : value;
        }

        public string GetCSVValue(string neTranslated)
        {
            return Value != null ? string.Join(Environment.NewLine, Value.Select(v => GetSynopticValue(v, neTranslated))) : string.Empty;
        }
    }
    #endregion HTML Helper Attributes
}