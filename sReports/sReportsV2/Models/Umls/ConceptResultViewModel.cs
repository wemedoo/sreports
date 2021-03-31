using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Umls
{
    public class ConceptResultViewModel
    {
        public string ClassType { get; set; }

        public string Ui { get; set; }

        public string Suppressible { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime MajorRevisionDate { get; set; }

        public string Status { get; set; }

        public List<SemanticTypeViewModel> SemanticTypes { get; set; }

        public int AtomCount { get; set; }

        public int AttributeCount { get; set; }

        public int CvMemberCount { get; set; }

        public string Atoms { get; set; }

        public string Definitions { get; set; }

        public string Relations { get; set; }

        public string DefaultPreferredAtom { get; set; }

        public int RelationCount { get; set; }

        public string Name { get; set; }
    }
}