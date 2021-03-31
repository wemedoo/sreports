using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs
{
    [DataContract]
    public class EnumDTO
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }
    }
}