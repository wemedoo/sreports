using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Telecom : TelecomBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("TelecomId")]
        public int TelecomId { get; set; }
        [Column("OrganizationId")]
        public int? OrganizationId { get; set; }
        [Column("ContactId")]
        public int? ContactId { get; set; }

        public Telecom() { }
        public Telecom(string system, string value, string use)
        {
            this.System = system;
            this.Value = value;
            this.Use = use;
        }
    }
}
