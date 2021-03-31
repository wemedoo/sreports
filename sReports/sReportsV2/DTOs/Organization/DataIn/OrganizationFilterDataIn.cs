using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Organization.DataIn
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

    }
}