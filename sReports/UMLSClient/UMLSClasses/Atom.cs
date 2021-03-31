using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class Atom
    {
        [JsonProperty("classType")]
        public string ClassType { get; set; }

        [JsonProperty("ui")]
        public string Ui { get; set; }

        [JsonProperty("suppressible")]
        public string Suppressible { get; set; }

        [JsonProperty("obsolete")]
        public string Obsolete { get; set; }

        [JsonProperty("rootSource")]
        public string RootSource { get; set; }

        [JsonProperty("termType")]
        public string TermType { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("concept")]
        public string Concept { get; set; }

        [JsonProperty("sourceConcept")]
        public string SourceConcept { get; set; }

        [JsonProperty("sourceDescriptor")]
        public string SourceDescriptor { get; set; }

        [JsonProperty("attributes")]
        public string Attributes { get; set; }

        [JsonProperty("parents")]
        public string Parents { get; set; }

        [JsonProperty("ancestors")]
        public string Ancestors { get; set; }

        [JsonProperty("children")]
        public string Children { get; set; }

        [JsonProperty("descendants")]
        public string Descendants { get; set; }

        [JsonProperty("relations")]
        public string Relations { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("contentViewMemberships")]
        public List<ContentViewMembership> ContentViewMemberships { get; set; }
    }
}
