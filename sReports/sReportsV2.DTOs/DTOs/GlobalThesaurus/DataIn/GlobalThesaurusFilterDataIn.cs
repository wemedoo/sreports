using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn
{
    public class GlobalThesaurusFilterDataIn : Common.DataIn
    {
        public string Term { get; set; }
        public string Language { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public string TermIndicator { get; set; }

    }
}
