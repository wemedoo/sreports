using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormAboutDataOut
    {
        public string PropertyOf { get; set; }
        public string FormatType { get; set; }
        public string FormatVersion { get; set; }
        public string Links { get; set; }
        public string ThesaurusReference { get; set; }
        public string ThesaurusVersion { get; set; }
        public string ThesaurusUpdated { get; set; }
    }
}