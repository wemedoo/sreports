using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.FormInstance.DataIn
{
    [DataContract]
    public class FormInstanceFilterDataIn
    {
        [DataMember(Name = "encounter")]
        public string Encounter { get; set; }

        [DataMember(Name = "performer")]
        public string Performer { get; set; }
    }
}
