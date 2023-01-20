using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class BodySurfaceCalculationFormula
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("BodySurfaceCalculationFormulaId")]
        public int BodySurfaceCalculationFormulaId { get; set; }
        public string Name { get; set; }
        public string Formula { get; set; }
    }
}
