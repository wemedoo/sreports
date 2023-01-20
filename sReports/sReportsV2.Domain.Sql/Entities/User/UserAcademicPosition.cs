using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class UserAcademicPosition : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("UserAcademicPositionId")]
        public int UserAcademicPositionIdId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int AcademicPositionTypeId { get; set; }
        [ForeignKey("AcademicPositionTypeId")]
        public AcademicPositionType AcademicPositionType { get; set; }

    }
}
