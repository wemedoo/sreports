using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class Indication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("IndicationId")]
        public int IndicationId { get; set; }
        public string Name { get; set; }
        public int ChemotherapySchemaId { get; set; }
        public bool IsDeleted { get; set; }
        public void Copy(Indication indication)
        {
            this.IndicationId = indication.IndicationId;
            this.Name = indication.Name;
        }
    }
}
