using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Umls.DatOut
{
    public class SearchResultDataOut
    {
        public string ClassType { get; set; }

        public List<ConceptSearchResultDataOut> Results { get; set; }
    }
}