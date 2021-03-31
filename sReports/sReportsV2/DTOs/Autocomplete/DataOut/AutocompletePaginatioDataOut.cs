using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Autocomplete
{
    public class AutocompletePaginatioDataOut
    {
        [JsonProperty("more")]
        public bool more { get; set; }
    }
}