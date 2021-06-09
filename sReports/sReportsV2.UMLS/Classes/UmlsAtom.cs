using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.UMLS.Classes
{
    public class UmlsAtom
    {
        //Atom unique identifier
        public string AUI { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Language { get; set; }
        public string Source { get; set; }
    }
}
