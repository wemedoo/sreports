using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class MedicationDose
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationDoseId")]
        public int MedicationDoseId { get; set; }
        public int DayNumber { get; set; }

        public List<MedicationDoseTime> MedicationDoseTimes { get; set; } = new List<MedicationDoseTime>();
        public bool IsDeleted { get; set; }
        public int? IntervalId { get; set; }
        public int? UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit Unit { get; set; }
        public int MedicationId { get; set; }

        public string GetStartTime()
        {
            if (MedicationDoseTimes.Count == 0 || MedicationDoseTimes.Where(t => !t.IsDeleted).FirstOrDefault() == null) return "";

            return MedicationDoseTimes.Where(t => !t.IsDeleted).FirstOrDefault().Time;
        }

        public void Copy(MedicationDose medicationDose)
        {
            this.DayNumber = medicationDose.DayNumber;
            this.IntervalId = medicationDose.IntervalId;
            this.UnitId = medicationDose.UnitId;

            UpdateMedicationDoseTimes(medicationDose.MedicationDoseTimes);
        }

        public void UpdateMedicationDoseTimes(List<MedicationDoseTime> newMedicationDoseTimes)
        {
            DeleteExistingRemovedDoseTimes(newMedicationDoseTimes);
            AddNewOrUpdateOldDoseTimes(newMedicationDoseTimes);
        }

        private void DeleteExistingRemovedDoseTimes(List<MedicationDoseTime> upcomingDoseTimes)
        {
            foreach (var medicationDoseTime in MedicationDoseTimes)
            {
                var remainingDoseTime = upcomingDoseTimes.Any(x => x.MedicationDoseTimeId == medicationDoseTime.MedicationDoseTimeId);
                if (!remainingDoseTime)
                {
                    medicationDoseTime.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldDoseTimes(List<MedicationDoseTime> upcomingDoseTimes)
        {
            foreach (var doseTime in upcomingDoseTimes)
            {
                if (doseTime.MedicationDoseTimeId == 0)
                {
                    MedicationDoseTimes.Add(doseTime);
                }
                else
                {
                    var dbDoseTime = MedicationDoseTimes.FirstOrDefault(x => x.MedicationDoseTimeId == doseTime.MedicationDoseTimeId && !x.IsDeleted);
                    if (dbDoseTime != null)
                    {
                        dbDoseTime.Copy(doseTime);
                    }
                }
            }
        }

    }
}
