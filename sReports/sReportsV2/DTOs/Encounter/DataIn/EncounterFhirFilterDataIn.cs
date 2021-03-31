using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Encounter
{
    [DataContract]
    public class EncounterFhirFilterDataIn
    {
        [DataMember(Name = "class")]
        public int Class { get; set; }

        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }
    }
}