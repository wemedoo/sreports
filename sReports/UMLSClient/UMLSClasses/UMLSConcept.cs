using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class UMLSConcept
    {
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("result")]
        public ConceptResult Result { get; set; }
    }
}
