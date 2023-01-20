using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.AccessManagment
{
    public class Module
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ModuleId")]
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<PermissionModule> Permissions { get; set; }
    }
}
