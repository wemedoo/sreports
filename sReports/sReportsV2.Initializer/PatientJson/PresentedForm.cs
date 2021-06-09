using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Initializer.PatientJson
{
    public class PresentedForm
    {
        [Index(0)]
        public string Content { get; set; }
    }
}
