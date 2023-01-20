using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CodeSystem
{
    public class CodeSystemDataOut
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string SAB { get; set; }
    }
}