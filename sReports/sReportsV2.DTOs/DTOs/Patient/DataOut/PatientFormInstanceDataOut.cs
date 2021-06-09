using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Patient.DataOut
{
    public class PatientFormInstanceDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string FormState { get; set; }
        public DateTime? Date { get; set; }
        public string ThesaurusId { get; set; }
        public string Language { get; set; }
    }
}