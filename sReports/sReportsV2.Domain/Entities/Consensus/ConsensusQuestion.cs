using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class ConsensusQuestion
    {
        public string ItemRef { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string Comment { get; set; }
        public string Value { get; set; }
    }
}
