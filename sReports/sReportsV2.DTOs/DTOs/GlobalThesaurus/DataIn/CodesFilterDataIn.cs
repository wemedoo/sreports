using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn
{
    public class CodesFilterDataIn : Common.DataIn
    {
        public int? Id { get; set; }
        public string ReturnUrl { get; set; }

        public List<string> CodeSystems { get; set; }
    }
}
