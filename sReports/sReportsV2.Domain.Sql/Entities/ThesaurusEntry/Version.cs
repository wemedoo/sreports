using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ThesaurusEntry
{
    public class Version
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("VersionId")]
        public int VersionId { get; set; }
        public VersionType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public ThesaurusState? State { get; set; }
        public AdministrativeData AdministrativeData {get;set;}
        public int AdministrativeDataId { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
    }
}
