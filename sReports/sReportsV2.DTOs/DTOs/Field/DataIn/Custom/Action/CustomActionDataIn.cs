using JsonSubTypes;
using Newtonsoft.Json;
using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn.Custom.Action
{
    [JsonConverter(typeof(JsonSubtypes), "ActionType")]
    [JsonSubtypes.KnownSubType(typeof(ControllerActionDataIn), CustomActionTypes.ControllerAction)]
    [JsonSubtypes.KnownSubType(typeof(JavascriptActionDataIn), CustomActionTypes.JavascriptAction)]
    public class CustomActionDataIn
    {
        public string ActionType { get; set; }
    }
}