using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Models
{
    public class ReviewModel
    {
        public string CTCAETerms { get; set; }
        public string Grades { get; set; }
        public string MedDRACode { get; set; }
        public string GradeDescription { get; set; }
    }
}
