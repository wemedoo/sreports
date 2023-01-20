using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.AccessManagment
{
    public class Permission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("PermissionId")]
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
