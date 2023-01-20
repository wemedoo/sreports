using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationClinicalDomain : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OrganizationClinicalDomainId")]
        public int OrganizationClinicalDomainId { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public int ClinicalDomainId { get; set; }
        [ForeignKey("ClinicalDomainId")]
        public ClinicalDomain ClinicalDomain { get; set; }
    }
}
