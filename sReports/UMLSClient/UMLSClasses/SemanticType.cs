using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class SemanticType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

    }
}
