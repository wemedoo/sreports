using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.DfD.DataOut
{
    public class DfDUserDataOut
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        public Gender Gender { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        MALE,
        FEMALE,
        UNKNOWN
    }
}
