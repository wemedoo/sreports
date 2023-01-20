using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class ClinicalDomain
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ClinicalDomainId")]
        public int ClinicalDomainId { get; set; }
        public string Name { get; set; }
    }
}
