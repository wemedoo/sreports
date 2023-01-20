using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn
{
    public class LiteratureReferenceDataIn
    {
        public int Id { get; set; }
        public string PubMedLink { get; set; }
        public int PubMedID { get; set; }
        public string ShortReferenceNotation { get; set; }
        public string DOI { get; set; }
        public DateTime? PublicationDate { get; set; }
        public int ChemotherapySchemaId { get; set; }
    }
}
