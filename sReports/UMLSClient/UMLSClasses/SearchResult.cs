using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class SearchResult
    {
        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("results")]
        public List<ConceptSearchResult> Results { get; set; }

    }
}
