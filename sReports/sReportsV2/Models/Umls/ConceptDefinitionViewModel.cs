using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Umls
{
    public class ConceptDefinitionViewModel
    {
        public string ClassType { get; set; }

        public string SourceOriginated { get; set; }

        public string RootSource { get; set; }

        public string Value { get; set; }
    }
}