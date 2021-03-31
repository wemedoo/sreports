using JsonSubTypes;
using Newtonsoft.Json;
using sReportsV2.Domain.Entities.Constants;
using sReportsV2.DTOs.Form.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(FieldCalculativeDataIn), FieldTypes.Calculative)]
    [JsonSubtypes.KnownSubType(typeof(FieldCheckboxDataIn), FieldTypes.Checkbox)]
    [JsonSubtypes.KnownSubType(typeof(FieldDateDataIn), FieldTypes.Date)]
    [JsonSubtypes.KnownSubType(typeof(FieldDatetimeDataIn), FieldTypes.Datetime)]
    [JsonSubtypes.KnownSubType(typeof(FieldNumericDataIn), FieldTypes.Number)]
    [JsonSubtypes.KnownSubType(typeof(FieldEmailDataIn), FieldTypes.Email)]
    [JsonSubtypes.KnownSubType(typeof(FieldFileDataIn), FieldTypes.File)]
    [JsonSubtypes.KnownSubType(typeof(FieldTextAreaDataIn), FieldTypes.LongText)]
    [JsonSubtypes.KnownSubType(typeof(FieldRadioDataIn), FieldTypes.Radio)]
    [JsonSubtypes.KnownSubType(typeof(FieldRegexDataIn), FieldTypes.Regex)]
    [JsonSubtypes.KnownSubType(typeof(FieldSelectDataIn), FieldTypes.Select)]
    [JsonSubtypes.KnownSubType(typeof(FieldTextDataIn), FieldTypes.Text)]
    public class FieldDataIn
    {
        public string InstanceId { get; set; }
        public string FhirType { get; set; }
        public string Id { get; set; }
        public List<string> Value { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string ThesaurusId { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsRequired { get; set; }
        public bool IsBold { get; set; }
        public FormHelpDataIn Help { get; set; }
        public bool IsHiddenOnPdf { get; set; }
    }
}