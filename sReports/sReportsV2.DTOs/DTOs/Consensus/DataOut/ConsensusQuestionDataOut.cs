using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class ConsensusQuestionDataOut
    {
        public string ItemRef { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string Comment { get; set; }
        public string Value { get; set; }


    }
}