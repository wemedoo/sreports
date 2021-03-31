using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfD.SMSApi.Client.DTOs.DataIn
{
    public class ValidationResultDataIn
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("statusCode")]
        public ValidationStatus StatusCode { get; set; }
    }

    public enum ValidationStatus
    {
        Valid,
        Invalid
    }
}
