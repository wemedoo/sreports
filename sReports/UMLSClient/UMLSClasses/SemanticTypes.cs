using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmlsClient.UMLSClasses
{
    public class SemanticTypes
    {
        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
