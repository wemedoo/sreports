using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceFhirFilter
    {
        public string Encounter { get; set; }
        public int Performer { get; set; }
    }
}
