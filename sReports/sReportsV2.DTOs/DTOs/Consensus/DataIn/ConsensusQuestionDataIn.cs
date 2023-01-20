using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class ConsensusQuestionDataIn
    {
        public string ItemRef { get; set; }
        public FormItemLevel Level { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string FormId { get; set; }
        public string IterationId { get; set; }
        public string Comment { get; set; }
        public string Value { get; set; }
    }
}