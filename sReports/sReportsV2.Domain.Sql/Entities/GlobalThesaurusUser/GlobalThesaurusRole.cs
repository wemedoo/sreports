using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser
{
    public class GlobalThesaurusRole : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("GlobalThesaurusRoleId")]
        public int GlobalThesaurusRoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
