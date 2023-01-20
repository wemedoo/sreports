using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class UserRole : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("UserRoleId")]
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
