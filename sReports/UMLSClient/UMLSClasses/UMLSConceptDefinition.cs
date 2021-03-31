using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UmlsClient.UMLSClasses;

namespace UMLSClient.UMLSClasses
{
    public class UMLSConceptDefinition
    {
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("result")]
        public List<ConceptDefinition> Result { get; set; }

    }
}
