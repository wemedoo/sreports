using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class FormAbout
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
