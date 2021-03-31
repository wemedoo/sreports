using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class ConceptSearchResult
    {
        [JsonProperty("ui")]
        public string Ui { get; set; }

        [JsonProperty("rootSource")]
        public string RootSource { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
