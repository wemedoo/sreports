using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DfD.DfDDataOut
{
    public class DfDFormTableDataOut
    {
        public string Id { get; set; }
        public string FormTitle { get; set; }
        public string FormId { get; set; }
        public string FormInstanceId { get; set; }
        public DateTime EntryDatetime { get; set; }
        public string ThesaurusId { get; set; }
    }
}