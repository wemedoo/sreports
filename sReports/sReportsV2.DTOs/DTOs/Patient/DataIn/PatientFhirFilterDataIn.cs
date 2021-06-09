using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace sReportsV2.DTOs.Patient.DataIn
{
    [DataContract]
    public class PatientFhirFilterDataIn
    {
        [DataMember(Name = "family")]
        public string Family { get; set; }

        [DataMember(Name = "given")]
        public string Given { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

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
    }
}