using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Umls
{
    public class SearchResultViewModel
    {
        public string ClassType { get; set; }

        public List<ConceptSearchResultViewModel> Results { get; set; }
    }
}