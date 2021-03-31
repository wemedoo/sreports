using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CTCAE.DataIn
{
    public class CTCAEPatient
    {
        public string PatientId { get; set; }
        public string VisitNo { get; set; }
        public DateTime? Date { get; set; }
        public List<ReviewModel> ReviewModels { get; set; } = new List<ReviewModel>();
        public string SelectedValue { get; set; }
    }
}