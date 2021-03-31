using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class TGT
    {
        [JsonProperty("TGTValue")]
        public string TGTValue { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }
    }
}
