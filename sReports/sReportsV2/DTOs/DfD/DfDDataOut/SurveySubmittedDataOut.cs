using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DfD.DfDDataOut
{
    public class SurveySubmittedDataOut
    {
        public string submittedDateTime { get; set; }
        public string surveyId { get; set; }
        public string userId { get; set; }
    }
}