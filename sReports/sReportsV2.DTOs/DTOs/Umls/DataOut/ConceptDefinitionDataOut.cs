using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Umls.DatOut
{
    public class ConceptDefinitionDataOut
    {
        public string ClassType { get; set; }

        public string SourceOriginated { get; set; }

        public string RootSource { get; set; }

        public string Value { get; set; }
    }
}