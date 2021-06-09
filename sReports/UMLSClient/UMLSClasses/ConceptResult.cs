using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class ConceptResult
    {
        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("ui")]
        public string Ui { get; set; }

        [JsonProperty("suppressible")]
        public string Suppressible { get; set; }

        [JsonProperty("dateAdded")]
        public DateTime DateAdded { get; set; }

        //[JsonProperty("majorRevisionDate")]
        //public DateTime MajorRevisionDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("semanticTypes")]
        public List<SemanticType> SemanticTypes { get; set; }

        [JsonProperty("atomCount")]
        public int AtomCount { get; set; }

        [JsonProperty("attributeCount")]
        public int AttributeCount { get; set; }

        [JsonProperty("cvMemberCount")]
        public int CvMemberCount { get; set; }

        [JsonProperty("atoms")]
        public string Atoms { get; set; }

        [JsonProperty("definitions")]
        public string Definitions { get; set; }

        [JsonProperty("relations")]
        public string Relations { get; set; }

        [JsonProperty("defaultPreferredAtom")]
        public string DefaultPreferredAtom { get; set; }

        [JsonProperty("relationCount")]
        public int RelationCount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
