using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReports.PathoLink.Entities
{
    public class CaseDetails
    {
        public string submissionID { get; set; }
        public string location { get; set; }
        public string submissionType { get; set; }
        public string probeID { get; set; }
        public string materialCode { get; set; }
        public string tissueCode { get; set; }
        public string birthday { get; set; }
        public string dateOfSurgery { get; set; }
        public string gender { get; set; }
    }
}
