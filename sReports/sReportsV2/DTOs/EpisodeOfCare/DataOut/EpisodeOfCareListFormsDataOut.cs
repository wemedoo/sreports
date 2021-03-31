using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareListFormsDataOut
    {
        public PatientDataOut Patient { get; set; }

        public EpisodeOfCareDataOut EpisodeOfCare { get; set; }

        public List<FormEpisodeOfCareDataOut> Forms { get; set; }

        public List<string> Referrals { get; set; }

        public string EncounterId { get; set; }
    }
}