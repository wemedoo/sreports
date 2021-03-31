using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class CTCAEModel
    {
        public string MedDraCode { get; set; }
        public string CTCAETerm { get; set; }
        public string Grade1 { get; set; }
        public string Grade2 { get; set; }
        public string Grade3 { get; set; }
        public string Grade4 { get; set; }
        public string Grade5 { get; set; }
        public string Grade { get; set; }
    }
}
