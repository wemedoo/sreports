using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class MedicationDoseTime
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationDoseTimeId")]
        public int MedicationDoseTimeId { get; set; }
        public string Time { get; set; }
        public string Dose { get; set; }
        public int MedicationDoseId { get; set; }
        public bool IsDeleted { get; set; }

        public void Copy(MedicationDoseTime medicationDoseTime)
        {
            this.Time = medicationDoseTime.Time;
            this.Dose = medicationDoseTime.Dose;
        }
    }
}
