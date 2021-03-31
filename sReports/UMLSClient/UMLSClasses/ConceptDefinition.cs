using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmlsClient.UMLSClasses
{
    public class ConceptDefinition
    {
        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("sourceOriginated")]
        public string SourceOriginated { get; set; }

        [JsonProperty("rootSource")]
        public string RootSource { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
