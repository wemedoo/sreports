using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.Model
{
    internal struct ElementValue
    {
        public ElementValue(string name, Base value)
        {
            ElementName = name;
            Value = value;
        }

        public string ElementName;
        public Base Value;
    }
}
