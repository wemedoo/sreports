using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class SelectItemModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public List<CTCAEModel> DefaultList { get; set; } = new List<CTCAEModel>();
    }
}
