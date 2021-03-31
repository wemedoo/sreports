using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    public class PatientFhirFilter
    {
        public string Family { get; set; }
        public string Given { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Telecom { get; set; }
        public string Identifier { get; set; }
    }
}
