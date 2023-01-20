using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormInstance.DataIn
{
    public class FormInstanceCovidFilterDataIn
    {
        public DateTime LastUpdate { get; set; }
        public int ThesaurusId { get; set; }
        public string FieldThesaurusId { get; set; }
    }
}