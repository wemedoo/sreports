using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Umls.DatOut
{
    public class AtomDataOut
    {
        public string ClassType { get; set; }

        public string Ui { get; set; }

        public string Suppressible { get; set; }

        public string Obsolete { get; set; }

        public string RootSource { get; set; }

        public string TermType { get; set; }

        public string Code { get; set; }

        public string Concept { get; set; }

        public string SourceConcept { get; set; }

        public string SourceDescriptor { get; set; }

        public string Attributes { get; set; }

        public string Parents { get; set; }

        public string Ancestors { get; set; }

        public string Children { get; set; }

        public string Descendants { get; set; }

        public string Relations { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }
    }
}