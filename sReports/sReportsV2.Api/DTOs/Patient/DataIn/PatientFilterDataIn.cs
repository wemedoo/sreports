using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace sReportsV2.Api.DTOs.Patient.DataIn
{
    [DataContract]
    public class PatientFilterDataIn
    {
        [DataMember(Name = "family")]
        public string Family { get; set; }

        [DataMember(Name = "given")]
        public string Given { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "postalcode")]
        public string PostalCode { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "telecom")]
        public string Telecom { get; set; }

        [DataMember(Name = "identifier")]
        public string Identifier { get; set; }
    }
}
