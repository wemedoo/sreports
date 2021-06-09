using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class SelectJsonModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public List<string> DefaultList { get; set; } = new List<string>();
    }
}
