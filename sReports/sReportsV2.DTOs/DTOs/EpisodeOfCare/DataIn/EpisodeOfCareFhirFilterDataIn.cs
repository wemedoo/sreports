using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    [DataContract]
    public class EpisodeOfCareFhirFilterDataIn
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "condition")]
        public string Condition { get; set; }

    }
}