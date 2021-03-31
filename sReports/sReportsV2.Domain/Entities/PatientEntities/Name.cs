using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    public class Name
    {
       public string Given { get; set; }
       public string Family { get; set; }

        public Name(string given, string family)
        {
            this.Given = given;
            this.Family = family;
        }
        public Name(){}
    }
}
