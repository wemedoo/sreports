using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.Organization.DataIn
{
    [DataContract]
    public class OrganizationFilterDataIn
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "partof")]
        public string PartOf { get; set; }

        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }
    }
}
