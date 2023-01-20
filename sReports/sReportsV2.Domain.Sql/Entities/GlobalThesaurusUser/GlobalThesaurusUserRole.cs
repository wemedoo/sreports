using sReportsV2.Domain.Sql.Entities.AccessManagment;
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
    public class GlobalThesaurusUserRole : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("GlobalThesaurusUserRoleId")]
        public int GlobalThesaurusUserRoleId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public GlobalThesaurusUser User { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public GlobalThesaurusRole Role { get; set; }
    }
}
