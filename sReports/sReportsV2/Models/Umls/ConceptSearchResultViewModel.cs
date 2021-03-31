using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Umls
{
    public class ConceptSearchResultViewModel
    {
        public string Ui { get; set; }

        public string RootSource { get; set; }

        public Uri Uri { get; set; }

        public string Name { get; set; }
    }
}