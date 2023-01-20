using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class MedicationInstance : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationInstanceId")]
        public int MedicationInstanceId { get; set; }
        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; }
        public int MedicationId { get; set; }
        public List<MedicationDoseInstance> MedicationDoses { get; set; } = new List<MedicationDoseInstance>();
        public int ChemotherapySchemaInstanceId { get; set; }

        public void Copy(MedicationInstance medication)
        {
            this.MedicationId = medication.MedicationId;
            
            UpdateMedicationDoses(medication.MedicationDoses);
        }

        public void UpdateMedicationDoses(List<MedicationDoseInstance> newMedicationDoses)
        {
            DeleteExistingRemovedDoses(newMedicationDoses);
            AddNewOrUpdateOldDoses(newMedicationDoses);
        }

        private void DeleteExistingRemovedDoses(List<MedicationDoseInstance> upcomingDoses)
        {
            foreach (var medicationDose in MedicationDoses)
            {
                var remainingDose = upcomingDoses.Any(x => x.MedicationDoseInstanceId == medicationDose.MedicationDoseInstanceId);
                if (!remainingDose)
                {
                    medicationDose.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldDoses(List<MedicationDoseInstance> upcomingDoses)
        {
            foreach (var dose in upcomingDoses)
            {
                if (dose.MedicationDoseInstanceId == 0)
                {
                    MedicationDoses.Add(dose);
                }
                else
                {
                    var dbDose = MedicationDoses.FirstOrDefault(x => x.MedicationDoseInstanceId == dose.MedicationDoseInstanceId && !x.IsDeleted);
                    if (dbDose != null)
                    {
                        dbDose.Copy(dose);
                    }
                }
            }
        }
    }
}
