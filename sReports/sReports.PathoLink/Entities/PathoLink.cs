using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReports.PathoLink.Entities
{
    public class PathoLink
    {
        public CaseDetails CaseDetails { get; set; }

        public List<PathoLinkField> ClinicalInformation { get; set; }

        public List<PathoLinkField> Result { get; set; }
    }
}
