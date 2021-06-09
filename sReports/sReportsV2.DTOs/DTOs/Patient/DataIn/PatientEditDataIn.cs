using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient.DataIn
{
    public class PatientEditDataIn
    {
        public int PatientId { get; set; }
        public int EpisodeOfCareId { get; set; }
        public int EncounterId { get; set; }
        public string FormInstanceId { get; set; }
    }
}