using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfD.SMSApi.Client.DTOs.DataOut
{
    public class InvalidateTokenDataOut
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
