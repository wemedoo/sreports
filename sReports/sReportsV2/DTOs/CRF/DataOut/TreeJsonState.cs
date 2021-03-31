using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CRF.DataOut
{
    public class TreeJsonState
    {
        public bool @checked { get; set; }
        public bool disabled { get; set; }
        public bool expandend { get; set; }
        public bool selected { get; set; }
    }
}