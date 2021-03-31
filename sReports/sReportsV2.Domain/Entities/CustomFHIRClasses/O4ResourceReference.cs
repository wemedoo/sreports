using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.CustomFHIRClasses
{
    public class O4ResourceReference
    {
        public O4Identifier Identifier { get; set; }
        public string Reference { get; set; }
        public O4ResourceReference() { }
        public O4ResourceReference(string reference, O4Identifier identifier)
        {
            Identifier = identifier;
            Reference = reference;
        }

    }
}
