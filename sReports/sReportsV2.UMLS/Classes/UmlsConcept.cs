using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.UMLS.Classes
{
    public class UmlsConcept
    {
        //Unique identifier for concept
        public string CUI { get; set; }
        public string RootSource { get; set; }
        public string TTY { get; set; }
        public List<UmlsAtom> Atoms { get; set; }
        
    }
}
