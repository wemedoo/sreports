using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    public class FormEpisodeOfCare
    {
        public string Status { get; set; }
        public string Type { get; set; }
        public string DiagnosisCondition { get; set; }
        public string DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
    }
}
