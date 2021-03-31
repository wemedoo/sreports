using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CRF.DataOut
{
    public class TreeJsonDataOut
    {
        public string text { get; set; }
        public bool selectable { get; set; }
        public string href { get; set; }
        public TreeJsonState state { get; set; }
        public List<TreeJsonDataOut> nodes { get; set; }
    }
}