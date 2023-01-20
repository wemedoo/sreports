using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class RouteOfAdministration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("RouteOfAdministrationId")]
        public int RouteOfAdministrationId { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string ShortName { get; set; }
        public string FDACode { get; set; }
        public string NCICondeptId { get; set; }

    }
}
