using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class UserConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int PageSize { get; set; } = 5;
        public string ActiveLanguage { get; set; }
        public string TimeZoneOffset { get; set; }
        public int? ActiveOrganizationId { get; set; }
        public virtual Organization ActiveOrganization { get; set; }
    }
}
