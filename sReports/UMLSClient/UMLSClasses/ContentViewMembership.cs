using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UMLSClient.UMLSClasses
{
    public class ContentViewMembership
    {
        [JsonProperty("memberUri")]
        public string MemberUri { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
