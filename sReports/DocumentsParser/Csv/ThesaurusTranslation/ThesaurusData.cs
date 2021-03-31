using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsParser.Csv.ThesaurusTranslation
{
    public class ThesaurusData
    {
        public string ItemNo { get; set; }
        public string InformationElements { get; set; }
        public string English { get; set; }
        public string UMLS { get; set; }
        public string GermanPreferredTerm { get; set; }
        public string FrancePreferredTerm { get; set; }
        public string SrpskiCrPreferredTerm { get; set; }
        public string SrpskiLtPreferredTerm { get; set; }
        public string RussianPreferredTerm { get; set; }
        public string SpanishPrefferedTerm { get; set; }
        public string PortuguesePrefferedTerm { get; set; }
        public string ThesaurusId { get; set; }
        public string DefinitionEnglish { get; set; }
        public string DefinitionGerman { get; set; }

        public string DefinitionFrance { get; set; }
        public string DefinitionSerbianCr { get; set; }
        public string DefinitionSerbianLt { get; set; }
        public string DefinitionRussian { get; set; }
        public string DefinitionSpanish { get; set; }
        public string DefinitionPortuguese { get; set; }
    }
}
