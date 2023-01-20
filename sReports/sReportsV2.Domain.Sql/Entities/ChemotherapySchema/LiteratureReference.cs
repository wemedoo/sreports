using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class LiteratureReference
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("LiteratureReferenceId")]
        public int LiteratureReferenceId { get; set; }
        public string PubMedLink { get; set; }
        public int PubMedID { get; set; }
        public string ShortReferenceNotation { get; set; }
        public string DOI { get; set; }
        public DateTime? PublicationDate { get; set; }
        public int ChemotherapySchemaId { get; set; }

        public void Copy(LiteratureReference literatureReference)
        {
            this.LiteratureReferenceId = literatureReference.LiteratureReferenceId;
            this.PubMedLink = literatureReference.PubMedLink;
            this.PubMedID = literatureReference.PubMedID;
            this.ShortReferenceNotation = literatureReference.ShortReferenceNotation;
            this.DOI = literatureReference.DOI;
            this.PublicationDate = literatureReference.PublicationDate;
        }

    }
}
