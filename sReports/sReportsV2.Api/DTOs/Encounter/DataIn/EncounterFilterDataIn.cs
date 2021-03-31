using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs
{
    [DataContract]
    public class EncounterFilterDataIn
    {
        [DataMember(Name = "class")]
        public int Class { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }
    }
}
