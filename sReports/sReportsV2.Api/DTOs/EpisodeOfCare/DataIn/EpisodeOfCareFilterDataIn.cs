using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.EpisodeOfCare.DataIn
{
    [DataContract]
    public class EpisodeOfCareFilterDataIn
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "condition")]
        public string Condition { get; set; }
    }
}
