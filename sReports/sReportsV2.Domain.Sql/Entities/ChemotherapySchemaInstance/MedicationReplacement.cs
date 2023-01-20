using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class MedicationReplacement : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationReplacementId")]
        public int MedicationReplacementId { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        [ForeignKey("ReplaceMedicationId")]
        public virtual MedicationInstance ReplaceMedication { get; set; }
        public int ReplaceMedicationId { get; set; }
        [ForeignKey("ReplaceWithMedicationId")]
        public virtual MedicationInstance ReplaceWithMedication { get; set; }
        public int ReplaceWithMedicationId { get; set; }
        [ForeignKey("CreatorId")]
        public virtual sReportsV2.Domain.Sql.Entities.User.User Creator { get; set; }
        public int CreatorId { get; set; }
    }
}
