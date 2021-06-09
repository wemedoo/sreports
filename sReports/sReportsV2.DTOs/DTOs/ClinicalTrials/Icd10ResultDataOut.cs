using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.ClinicalTrials
{
    public class Icd10ResultDataOut
    {
        public List<string> Icd10Codes { get; set; }
        public List<string> SimilarTerms { get; set; }
        public Icd10ResultDataOut() 
        {
            Icd10Codes = new List<string>();
            SimilarTerms = new List<string>();
        }
    }
}