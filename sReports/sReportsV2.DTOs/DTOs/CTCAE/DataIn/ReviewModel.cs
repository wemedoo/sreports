using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.CTCAE.DataIn
{
    public class ReviewModel
    {
        public string CTCAETerms { get; set; }
        public string Grades { get; set; }
        public string MedDRACode { get; set; }
        public string GradeDescription { get; set; }
    }
}